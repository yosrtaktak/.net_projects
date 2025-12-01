using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.Application.Factories;

namespace Backend.Application.Services;

public interface IRentalService
{
    Task<Rental> CreateRentalAsync(int customerId, int vehicleId, DateTime startDate, DateTime endDate, string pricingStrategy = "standard");
    Task<Rental?> GetRentalByIdAsync(int rentalId);
    Task<IEnumerable<Rental>> GetAllRentalsAsync();
    Task<IEnumerable<Rental>> GetRentalsByCustomerAsync(int customerId);
    Task<Rental> CompleteRentalAsync(int rentalId, int endMileage);
    Task<Rental> CancelRentalAsync(int rentalId);
    Task<decimal> CalculatePriceAsync(int vehicleId, int customerId, DateTime startDate, DateTime endDate, string pricingStrategy = "standard");
}

public class RentalService : IRentalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRentalRepository _rentalRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IPricingStrategyFactory _pricingStrategyFactory;

    public RentalService(
        IUnitOfWork unitOfWork,
        IRentalRepository rentalRepository,
        IVehicleRepository vehicleRepository,
        IPricingStrategyFactory pricingStrategyFactory)
    {
        _unitOfWork = unitOfWork;
        _rentalRepository = rentalRepository;
        _vehicleRepository = vehicleRepository;
        _pricingStrategyFactory = pricingStrategyFactory;
    }

    public async Task<Rental> CreateRentalAsync(int customerId, int vehicleId, DateTime startDate, DateTime endDate, string pricingStrategy = "standard")
    {
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

        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(customerId);
        if (customer == null)
        {
            throw new ArgumentException("Customer not found.");
        }

        // Calculate price using the selected strategy
        var price = await CalculatePriceAsync(vehicleId, customerId, startDate, endDate, pricingStrategy);

        var rental = new Rental
        {
            CustomerId = customerId,
            VehicleId = vehicleId,
            StartDate = startDate,
            EndDate = endDate,
            TotalCost = price,
            Status = RentalStatus.Reserved,
            CreatedAt = DateTime.UtcNow,
            StartMileage = vehicle.Mileage
        };

        await _rentalRepository.AddAsync(rental);
        
        // Update vehicle status
        vehicle.Status = VehicleStatus.Rented;
        _vehicleRepository.Update(vehicle);
        
        await _unitOfWork.CommitAsync();

        return rental;
    }

    public async Task<Rental?> GetRentalByIdAsync(int rentalId)
    {
        return await _rentalRepository.GetRentalWithDetailsAsync(rentalId);
    }

    public async Task<IEnumerable<Rental>> GetAllRentalsAsync()
    {
        return await _rentalRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Rental>> GetRentalsByCustomerAsync(int customerId)
    {
        return await _rentalRepository.GetRentalsByCustomerAsync(customerId);
    }

    public async Task<Rental> CompleteRentalAsync(int rentalId, int endMileage)
    {
        var rental = await _rentalRepository.GetRentalWithDetailsAsync(rentalId);
        if (rental == null)
        {
            throw new ArgumentException("Rental not found.");
        }

        rental.Status = RentalStatus.Completed;
        rental.ActualReturnDate = DateTime.UtcNow;
        rental.EndMileage = endMileage;

        _rentalRepository.Update(rental);

        // Update vehicle status and mileage
        var vehicle = rental.Vehicle;
        vehicle.Status = VehicleStatus.Available;
        vehicle.Mileage = endMileage;
        _vehicleRepository.Update(vehicle);

        await _unitOfWork.CommitAsync();

        return rental;
    }

    public async Task<Rental> CancelRentalAsync(int rentalId)
    {
        var rental = await _rentalRepository.GetRentalWithDetailsAsync(rentalId);
        if (rental == null)
        {
            throw new ArgumentException("Rental not found.");
        }

        rental.Status = RentalStatus.Cancelled;
        _rentalRepository.Update(rental);

        // Update vehicle status back to available
        var vehicle = rental.Vehicle;
        vehicle.Status = VehicleStatus.Available;
        _vehicleRepository.Update(vehicle);

        await _unitOfWork.CommitAsync();

        return rental;
    }

    public async Task<decimal> CalculatePriceAsync(int vehicleId, int customerId, DateTime startDate, DateTime endDate, string pricingStrategy = "standard")
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle == null)
        {
            throw new ArgumentException("Vehicle not found.");
        }

        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(customerId);
        if (customer == null)
        {
            throw new ArgumentException("Customer not found.");
        }

        var strategy = _pricingStrategyFactory.CreateStrategy(pricingStrategy);
        return strategy.CalculatePrice(vehicle, startDate, endDate, customer);
    }
}
