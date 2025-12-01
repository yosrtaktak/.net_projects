using Backend.Core.Entities;

namespace Backend.Core.Interfaces;

public interface IMaintenanceRepository : IRepository<Maintenance>
{
    Task<IEnumerable<Maintenance>> GetMaintenancesByVehicleAsync(int vehicleId);
    Task<IEnumerable<Maintenance>> GetMaintenancesByStatusAsync(MaintenanceStatus status);
    Task<IEnumerable<Maintenance>> GetMaintenancesByTypeAsync(MaintenanceType type);
    Task<IEnumerable<Maintenance>> GetOverdueMaintenancesAsync();
    Task<IEnumerable<Maintenance>> GetScheduledMaintenancesAsync(DateTime startDate, DateTime endDate);
    Task<Maintenance?> GetByIdWithVehicleAsync(int id);
}
