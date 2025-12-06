using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Backend.Core.Entities;
using Backend.Application.DTOs;
using Backend.Application.Services;
using Backend.Core.Interfaces;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public RentalsController(
        IRentalService rentalService,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager)
    {
        _rentalService = rentalService;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAllRentals()
    {
        var rentals = await _rentalService.GetAllRentalsAsync();
        var rentalDtos = rentals.Select(r => MapRentalToDto(r));
        return Ok(rentalDtos);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("manage")]
    public async Task<ActionResult<IEnumerable<object>>> GetRentalsForManagement(
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? vehicleId = null,
        [FromQuery] string? userId = null)
    {
        try
        {
            var rentals = await _rentalService.GetRentalsForManagementAsync(
                status, startDate, endDate, vehicleId, userId);
            var rentalDtos = rentals.Select(r => MapRentalToDto(r));
            return Ok(rentalDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error fetching rentals: {ex.Message}" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetRental(int id)
    {
        var rental = await _rentalService.GetRentalByIdAsync(id);
        
        if (rental == null)
        {
            return NotFound(new { message = "Rental not found" });
        }

        return Ok(MapRentalToDto(rental));
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<object>>> GetRentalsByUser(string userId)
    {
        var rentals = await _rentalService.GetRentalsByUserAsync(userId);
        var rentalDtos = rentals.Select(r => MapRentalToDto(r));
        return Ok(rentalDtos);
    }

    [HttpPost]
    public async Task<ActionResult<object>> CreateRental([FromBody] CreateRentalDto dto)
    {
        try
        {
            string userId;
            
            // If customer role, get their user ID from authentication
            if (User.IsInRole("Customer"))
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return NotFound(new { message = "User profile not found" });
                }

                // Override the userId with the authenticated user's ID
                userId = user.Id;
            }
            else
            {
                // Admin/Employee can specify any user
                userId = dto.UserId;
            }

            var rental = await _rentalService.CreateRentalAsync(
                userId, 
                dto.VehicleId, 
                dto.StartDate, 
                dto.EndDate, 
                dto.PricingStrategy);

            return CreatedAtAction(nameof(GetRental), new { id = rental.Id }, MapRentalToDto(rental));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("calculate-price")]
    public async Task<ActionResult<PriceCalculationResponse>> CalculatePrice([FromBody] CalculatePriceDto dto)
    {
        try
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId);
            if (vehicle == null)
            {
                return BadRequest(new { message = "Vehicle not found" });
            }

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            var totalPrice = await _rentalService.CalculatePriceAsync(
                dto.VehicleId, 
                dto.UserId, 
                dto.StartDate, 
                dto.EndDate, 
                dto.PricingStrategy);

            var numberOfDays = (dto.EndDate - dto.StartDate).Days;
            
            // Calculate discount if applicable
            decimal? discount = null;
            if (dto.PricingStrategy != "standard")
            {
                var standardPrice = vehicle.DailyRate * numberOfDays;
                if (totalPrice < standardPrice)
                {
                    discount = Math.Round(((standardPrice - totalPrice) / standardPrice) * 100, 2);
                }
            }

            var response = new PriceCalculationResponse
            {
                TotalPrice = totalPrice,
                NumberOfDays = numberOfDays,
                DailyRate = vehicle.DailyRate,
                StrategyUsed = dto.PricingStrategy,
                Discount = discount
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}/complete")]
    public async Task<ActionResult<object>> CompleteRental(int id, [FromBody] CompleteRentalDto dto)
    {
        try
        {
            var rental = await _rentalService.CompleteRentalAsync(id, dto.EndMileage);
            return Ok(MapRentalToDto(rental));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<object>> CancelRental(int id)
    {
        try
        {
            // If customer role, verify they own the rental
            if (User.IsInRole("Customer"))
            {
                var rental = await _rentalService.GetRentalByIdAsync(id);
                if (rental == null)
                {
                    return NotFound(new { message = "Rental not found" });
                }

                var userEmail = User.Identity?.Name;
                var currentUser = await _userManager.FindByEmailAsync(userEmail ?? "");
                
                if (currentUser == null || rental.UserId != currentUser.Id)
                {
                    return Forbid();
                }

                // Customers can only cancel Reserved rentals
                if (rental.Status != RentalStatus.Reserved)
                {
                    return BadRequest(new { message = "Only reserved rentals can be cancelled" });
                }
            }

            var updatedRental = await _rentalService.CancelRentalAsync(id);
            return Ok(MapRentalToDto(updatedRental));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}/status")]
    public async Task<ActionResult<object>> UpdateRentalStatus(int id, [FromBody] UpdateRentalStatusDto dto)
    {
        try
        {
            var rental = await _rentalService.UpdateRentalStatusAsync(id, dto.Status);
            return Ok(MapRentalToDto(rental));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private object MapRentalToDto(Rental rental)
    {
        return new
        {
            rental.Id,
            rental.UserId,
            rental.VehicleId,
            rental.StartDate,
            rental.EndDate,
            rental.ActualReturnDate,
            rental.TotalCost,
            rental.Status,
            rental.StartMileage,
            rental.EndMileage,
            rental.Notes,
            rental.CreatedAt,
            Customer = rental.User != null ? new
            {
                Id = rental.User.Id,
                FirstName = rental.User.FirstName,
                LastName = rental.User.LastName,
                Email = rental.User.Email,
                PhoneNumber = rental.User.PhoneNumber,
                DriverLicenseNumber = rental.User.DriverLicenseNumber,
                DateOfBirth = rental.User.DateOfBirth,
                Address = rental.User.Address,
                RegistrationDate = rental.User.RegistrationDate,
                Tier = rental.User.Tier
            } : null,
            Vehicle = rental.Vehicle != null ? new
            {
                rental.Vehicle.Id,
                rental.Vehicle.Brand,
                rental.Vehicle.Model,
                rental.Vehicle.Year,
                rental.Vehicle.Category,
                rental.Vehicle.DailyRate,
                rental.Vehicle.Status,
                LicensePlate = rental.Vehicle.RegistrationNumber,
                rental.Vehicle.Mileage,
                rental.Vehicle.ImageUrl
            } : null,
            Payment = rental.Payment != null ? new
            {
                rental.Payment.Id,
                rental.Payment.Amount,
                rental.Payment.PaymentDate,
                PaymentMethod = rental.Payment.Method,
                rental.Payment.TransactionId
            } : null
        };
    }
}
