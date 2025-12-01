using Microsoft.EntityFrameworkCore;
using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.Infrastructure.Data;

namespace Backend.Infrastructure.Repositories;

public class RentalRepository : Repository<Rental>, IRentalRepository
{
    public RentalRepository(CarRentalDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
    {
        return await _dbSet
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Where(r => r.Status == RentalStatus.Active)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetRentalsByCustomerAsync(int customerId)
    {
        return await _dbSet
            .Include(r => r.Vehicle)
            .Include(r => r.Payment)
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetRentalsByVehicleAsync(int vehicleId)
    {
        return await _dbSet
            .Include(r => r.Customer)
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
    }

    public async Task<Rental?> GetRentalWithDetailsAsync(int rentalId)
    {
        return await _dbSet
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .Include(r => r.Payment)
            .FirstOrDefaultAsync(r => r.Id == rentalId);
    }

    public async Task<Rental?> GetRentalWithCustomerAsync(int rentalId)
    {
        return await _dbSet
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == rentalId);
    }

    public async Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startDate, DateTime endDate)
    {
        return !await _dbSet.AnyAsync(r =>
            r.VehicleId == vehicleId &&
            r.Status != RentalStatus.Cancelled &&
            r.StartDate < endDate &&
            r.EndDate > startDate);
    }
}
