using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Core.Entities;
using Backend.Core.Interfaces;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
    {
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(id);
        
        if (customer == null)
        {
            return NotFound(new { message = "Customer not found" });
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer([FromBody] Customer customer)
    {
        customer.RegistrationDate = DateTime.UtcNow;
        await _unitOfWork.Repository<Customer>().AddAsync(customer);
        await _unitOfWork.CommitAsync();

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Customer>> UpdateCustomer(int id, [FromBody] Customer customer)
    {
        var existingCustomer = await _unitOfWork.Repository<Customer>().GetByIdAsync(id);
        
        if (existingCustomer == null)
        {
            return NotFound(new { message = "Customer not found" });
        }

        existingCustomer.FirstName = customer.FirstName;
        existingCustomer.LastName = customer.LastName;
        existingCustomer.Email = customer.Email;
        existingCustomer.PhoneNumber = customer.PhoneNumber;
        existingCustomer.DriverLicenseNumber = customer.DriverLicenseNumber;
        existingCustomer.DateOfBirth = customer.DateOfBirth;
        existingCustomer.Address = customer.Address;
        existingCustomer.Tier = customer.Tier;

        _unitOfWork.Repository<Customer>().Update(existingCustomer);
        await _unitOfWork.CommitAsync();

        return Ok(existingCustomer);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCustomer(int id)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(id);
        
        if (customer == null)
        {
            return NotFound(new { message = "Customer not found" });
        }

        _unitOfWork.Repository<Customer>().Remove(customer);
        await _unitOfWork.CommitAsync();

        return NoContent();
    }
}
