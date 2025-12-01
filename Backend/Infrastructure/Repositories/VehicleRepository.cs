using Microsoft.EntityFrameworkCore;
using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.Infrastructure.Data;

namespace Backend.Infrastructure.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(CarRentalDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(v => v.Status == VehicleStatus.Available &&
                   !v.Rentals.Any(r =>
                       r.Status == RentalStatus.Active &&
                       r.StartDate < endDate &&
                       r.EndDate > startDate))
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetVehiclesByCategoryAsync(VehicleCategory category)
    {
        return await _dbSet
            .Where(v => v.Category == category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetVehiclesByStatusAsync(VehicleStatus status)
    {
        return await _dbSet
            .Where(v => v.Status == status)
            .ToListAsync();
    }

    public async Task<Vehicle?> GetVehicleWithRentalsAsync(int vehicleId)
    {
        return await _dbSet
            .Include(v => v.Rentals)
            .ThenInclude(r => r.Customer)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);
    }

    public async Task<Vehicle?> GetByIdWithHistoryAsync(int vehicleId)
    {
        return await _dbSet
            .Include(v => v.Rentals)
                .ThenInclude(r => r.Customer)
            .Include(v => v.MaintenanceRecords)
            .Include(v => v.DamageRecords)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);
    }
}
