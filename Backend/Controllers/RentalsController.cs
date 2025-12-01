using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Core.Entities;
using Backend.Application.DTOs;
using Backend.Application.Services;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentalsController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rental>>> GetAllRentals()
    {
        var rentals = await _rentalService.GetAllRentalsAsync();
        return Ok(rentals);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Rental>> GetRental(int id)
    {
        var rental = await _rentalService.GetRentalByIdAsync(id);
        
        if (rental == null)
        {
            return NotFound(new { message = "Rental not found" });
        }

        return Ok(rental);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<Rental>>> GetRentalsByCustomer(int customerId)
    {
        var rentals = await _rentalService.GetRentalsByCustomerAsync(customerId);
        return Ok(rentals);
    }

    [HttpPost]
    public async Task<ActionResult<Rental>> CreateRental([FromBody] CreateRentalDto dto)
    {
        try
        {
            var rental = await _rentalService.CreateRentalAsync(
                dto.CustomerId, 
                dto.VehicleId, 
                dto.StartDate, 
                dto.EndDate, 
                dto.PricingStrategy);

            return CreatedAtAction(nameof(GetRental), new { id = rental.Id }, rental);
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
    public async Task<ActionResult<decimal>> CalculatePrice([FromBody] CalculatePriceDto dto)
    {
        try
        {
            var price = await _rentalService.CalculatePriceAsync(
                dto.VehicleId, 
                dto.CustomerId, 
                dto.StartDate, 
                dto.EndDate, 
                dto.PricingStrategy);

            return Ok(new { price, pricingStrategy = dto.PricingStrategy });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}/complete")]
    public async Task<ActionResult<Rental>> CompleteRental(int id, [FromBody] CompleteRentalDto dto)
    {
        try
        {
            var rental = await _rentalService.CompleteRentalAsync(id, dto.EndMileage);
            return Ok(rental);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<Rental>> CancelRental(int id)
    {
        try
        {
            var rental = await _rentalService.CancelRentalAsync(id);
            return Ok(rental);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
