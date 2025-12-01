using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class VehicleDamageRepository : Repository<VehicleDamage>, IVehicleDamageRepository
{
    private readonly CarRentalDbContext _context;

    public VehicleDamageRepository(CarRentalDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VehicleDamage>> GetDamagesByVehicleAsync(int vehicleId)
    {
        return await _context.VehicleDamages
            .Where(d => d.VehicleId == vehicleId)
            .Include(d => d.Vehicle)
            .Include(d => d.Rental)
                .ThenInclude(r => r!.Customer)
            .OrderByDescending(d => d.ReportedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<VehicleDamage>> GetDamagesByRentalAsync(int rentalId)
    {
        return await _context.VehicleDamages
            .Where(d => d.RentalId == rentalId)
            .Include(d => d.Vehicle)
            .Include(d => d.Rental)
                .ThenInclude(r => r!.Customer)
            .OrderByDescending(d => d.ReportedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<VehicleDamage>> GetDamagesBySeverityAsync(DamageSeverity severity)
    {
        return await _context.VehicleDamages
            .Where(d => d.Severity == severity)
            .Include(d => d.Vehicle)
            .Include(d => d.Rental)
            .OrderByDescending(d => d.ReportedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<VehicleDamage>> GetDamagesByStatusAsync(DamageStatus status)
    {
        return await _context.VehicleDamages
            .Where(d => d.Status == status)
            .Include(d => d.Vehicle)
            .Include(d => d.Rental)
            .OrderByDescending(d => d.ReportedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<VehicleDamage>> GetUnresolvedDamagesAsync()
    {
        return await _context.VehicleDamages
            .Where(d => d.Status != DamageStatus.Repaired)
            .Include(d => d.Vehicle)
            .Include(d => d.Rental)
            .OrderByDescending(d => d.ReportedDate)
            .ToListAsync();
    }

    public async Task<VehicleDamage?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.VehicleDamages
            .Include(d => d.Vehicle)
            .Include(d => d.Rental)
                .ThenInclude(r => r!.Customer)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<VehicleDamage>> GetAllWithDetailsAsync()
    {
        return await _context.VehicleDamages
            .Include(d => d.Vehicle)
            .Include(d => d.Rental)
            .OrderByDescending(d => d.ReportedDate)
            .ToListAsync();
    }
}
