using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.Application.Factories;
using Microsoft.AspNetCore.Identity;

namespace Backend.Application.Services;

public interface IRentalService
{
    Task<Rental> CreateRentalAsync(string userId, int vehicleId, DateTime startDate, DateTime endDate, string pricingStrategy = "standard");
    Task<Rental?> GetRentalByIdAsync(int rentalId);
    Task<IEnumerable<Rental>> GetAllRentalsAsync();
    Task<IEnumerable<Rental>> GetRentalsByUserAsync(string userId);
    Task<IEnumerable<Rental>> GetRentalsForManagementAsync(string? status, DateTime? startDate, DateTime? endDate, int? vehicleId, string? userId);
    Task<Rental> CompleteRentalAsync(int rentalId, int endMileage);
    Task<Rental> CancelRentalAsync(int rentalId);
    Task<Rental> UpdateRentalStatusAsync(int rentalId, string status);
    Task<decimal> CalculatePriceAsync(int vehicleId, string userId, DateTime startDate, DateTime endDate, string pricingStrategy = "standard");
}

public class RentalService : IRentalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRentalRepository _rentalRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IPricingStrategyFactory _pricingStrategyFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    public RentalService(
        IUnitOfWork unitOfWork,
        IRentalRepository rentalRepository,
        IVehicleRepository vehicleRepository,
        IPricingStrategyFactory pricingStrategyFactory,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _rentalRepository = rentalRepository;
        _vehicleRepository = vehicleRepository;
        _pricingStrategyFactory = pricingStrategyFactory;
        _userManager = userManager;
    }

    public async Task<Rental> CreateRentalAsync(string userId, int vehicleId, DateTime startDate, DateTime endDate, string pricingStrategy = "standard")
    {
        // Check user exists and has Customer role
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains("Customer"))
        {
            throw new ArgumentException("User is not a customer.");
        }

        // Check vehicle availability
        var isAvailable = await _rentalRepository.IsVehicleAvailableAsync(vehicleId, startDate, endDate);
        if (!isAvailable)
        {
            throw new InvalidOperationException("Vehicle is not available for the selected dates.");
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle == null)
        {
            throw new ArgumentException("Vehicle not found.");
        }

        // Calculate price using the selected strategy
        var price = await CalculatePriceAsync(vehicleId, userId, startDate, endDate, pricingStrategy);

        var rental = new Rental
        {
            UserId = userId,
            VehicleId = vehicleId,
            StartDate = startDate,
            EndDate = endDate,
            TotalCost = price,
            Status = RentalStatus.Reserved,
            CreatedAt = DateTime.UtcNow
        };

        await _rentalRepository.AddAsync(rental);

        // Update vehicle status
        vehicle.Status = VehicleStatus.Rented;
        _vehicleRepository.Update(vehicle);

        await _unitOfWork.CommitAsync();

        // Load navigation properties
        var createdRental = await _rentalRepository.GetByIdWithDetailsAsync(rental.Id);
        return createdRental ?? rental;
    }

    public async Task<Rental?> GetRentalByIdAsync(int rentalId)
    {
        return await _rentalRepository.GetByIdWithDetailsAsync(rentalId);
    }

    public async Task<IEnumerable<Rental>> GetAllRentalsAsync()
    {
        return await _rentalRepository.GetAllWithDetailsAsync();
    }

    public async Task<IEnumerable<Rental>> GetRentalsByUserAsync(string userId)
    {
        return await _rentalRepository.GetRentalsByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Rental>> GetRentalsForManagementAsync(
        string? status, 
        DateTime? startDate, 
        DateTime? endDate, 
        int? vehicleId, 
        string? userId)
    {
        return await _rentalRepository.GetRentalsForManagementAsync(status, startDate, endDate, vehicleId, userId);
    }

    public async Task<Rental> CompleteRentalAsync(int rentalId, int endMileage)
    {
        var rental = await _rentalRepository.GetByIdWithDetailsAsync(rentalId);
        if (rental == null)
        {
            throw new ArgumentException("Rental not found.");
        }

        if (rental.Status != RentalStatus.Active)
        {
            throw new InvalidOperationException("Only active rentals can be completed.");
        }

        rental.Status = RentalStatus.Completed;
        rental.ActualReturnDate = DateTime.UtcNow;
        rental.EndMileage = endMileage;

        _rentalRepository.Update(rental);

        // Update vehicle status
        if (rental.Vehicle != null)
        {
            rental.Vehicle.Status = VehicleStatus.Available;
            rental.Vehicle.Mileage = endMileage;
            _vehicleRepository.Update(rental.Vehicle);
        }

        await _unitOfWork.CommitAsync();

        return rental;
    }

    public async Task<Rental> CancelRentalAsync(int rentalId)
    {
        var rental = await _rentalRepository.GetByIdWithDetailsAsync(rentalId);
        if (rental == null)
        {
            throw new ArgumentException("Rental not found.");
        }

        rental.Status = RentalStatus.Cancelled;
        _rentalRepository.Update(rental);

        // Update vehicle status
        if (rental.Vehicle != null && rental.Vehicle.Status == VehicleStatus.Rented)
        {
            rental.Vehicle.Status = VehicleStatus.Available;
            _vehicleRepository.Update(rental.Vehicle);
        }

        await _unitOfWork.CommitAsync();

        return rental;
    }

    public async Task<Rental> UpdateRentalStatusAsync(int rentalId, string status)
    {
        var rental = await _rentalRepository.GetByIdWithDetailsAsync(rentalId);
        if (rental == null)
        {
            throw new ArgumentException("Rental not found.");
        }

        if (!Enum.TryParse<RentalStatus>(status, true, out var rentalStatus))
        {
            throw new ArgumentException($"Invalid rental status: {status}");
        }

        rental.Status = rentalStatus;
        _rentalRepository.Update(rental);

        // Update vehicle status based on rental status
        var vehicle = rental.Vehicle;
        if (vehicle != null)
        {
            vehicle.Status = rentalStatus switch
            {
                RentalStatus.Active or RentalStatus.Reserved => VehicleStatus.Rented,
                RentalStatus.Completed or RentalStatus.Cancelled => VehicleStatus.Available,
                _ => vehicle.Status
            };
            _vehicleRepository.Update(vehicle);
        }

        await _unitOfWork.CommitAsync();

        return rental;
    }

    public async Task<decimal> CalculatePriceAsync(int vehicleId, string userId, DateTime startDate, DateTime endDate, string pricingStrategy = "standard")
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle == null)
        {
            throw new ArgumentException("Vehicle not found.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        var strategy = _pricingStrategyFactory.CreateStrategy(pricingStrategy);
        return strategy.CalculatePrice(vehicle, startDate, endDate, user);
    }
}
