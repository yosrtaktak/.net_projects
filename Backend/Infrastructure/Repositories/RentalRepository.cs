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
            .Include(r => r.User)
            .Include(r => r.Vehicle)
            .Where(r => r.Status == RentalStatus.Active)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetRentalsByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(r => r.Vehicle)
            .Include(r => r.Payment)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetRentalsByVehicleAsync(int vehicleId)
    {
        return await _dbSet
            .Include(r => r.User)
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
    }

    public async Task<Rental?> GetByIdWithDetailsAsync(int rentalId)
    {
        return await _dbSet
            .Include(r => r.User)
            .Include(r => r.Vehicle)
            .Include(r => r.Payment)
            .FirstOrDefaultAsync(r => r.Id == rentalId);
    }

    public async Task<IEnumerable<Rental>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(r => r.User)
            .Include(r => r.Vehicle)
            .Include(r => r.Payment)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startDate, DateTime endDate)
    {
        return !await _dbSet.AnyAsync(r =>
            r.VehicleId == vehicleId &&
            r.Status != RentalStatus.Cancelled &&
            r.StartDate < endDate &&
            r.EndDate > startDate);
    }

    public async Task<IEnumerable<Rental>> GetRentalsForManagementAsync(
        string? status, 
        DateTime? startDate, 
        DateTime? endDate, 
        int? vehicleId, 
        string? userId)
    {
        var query = _dbSet
            .Include(r => r.User)
            .Include(r => r.Vehicle)
            .Include(r => r.Payment)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<RentalStatus>(status, true, out var rentalStatus))
        {
            query = query.Where(r => r.Status == rentalStatus);
        }

        if (startDate.HasValue)
        {
            query = query.Where(r => r.StartDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(r => r.EndDate <= endDate.Value);
        }

        if (vehicleId.HasValue)
        {
            query = query.Where(r => r.VehicleId == vehicleId.Value);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(r => r.UserId == userId);
        }

        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }
}
