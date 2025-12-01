using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class MaintenanceRepository : Repository<Maintenance>, IMaintenanceRepository
{
    private readonly CarRentalDbContext _context;

    public MaintenanceRepository(CarRentalDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Maintenance>> GetMaintenancesByVehicleAsync(int vehicleId)
    {
        return await _context.Maintenances
            .Where(m => m.VehicleId == vehicleId)
            .Include(m => m.Vehicle)
            .OrderByDescending(m => m.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Maintenance>> GetMaintenancesByStatusAsync(MaintenanceStatus status)
    {
        return await _context.Maintenances
            .Where(m => m.Status == status)
            .Include(m => m.Vehicle)
            .OrderByDescending(m => m.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Maintenance>> GetMaintenancesByTypeAsync(MaintenanceType type)
    {
        return await _context.Maintenances
            .Where(m => m.Type == type)
            .Include(m => m.Vehicle)
            .OrderByDescending(m => m.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Maintenance>> GetOverdueMaintenancesAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Maintenances
            .Where(m => m.Status == MaintenanceStatus.Scheduled && m.ScheduledDate < today)
            .Include(m => m.Vehicle)
            .OrderBy(m => m.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Maintenance>> GetScheduledMaintenancesAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Maintenances
            .Where(m => m.ScheduledDate >= startDate && m.ScheduledDate <= endDate)
            .Include(m => m.Vehicle)
            .OrderBy(m => m.ScheduledDate)
            .ToListAsync();
    }

    public async Task<Maintenance?> GetByIdWithVehicleAsync(int id)
    {
        return await _context.Maintenances
            .Include(m => m.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}
