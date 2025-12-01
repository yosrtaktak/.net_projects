using Backend.Core.Entities;

namespace Backend.Core.Interfaces;

public interface IVehicleDamageRepository : IRepository<VehicleDamage>
{
    Task<IEnumerable<VehicleDamage>> GetDamagesByVehicleAsync(int vehicleId);
    Task<IEnumerable<VehicleDamage>> GetDamagesByRentalAsync(int rentalId);
    Task<IEnumerable<VehicleDamage>> GetDamagesBySeverityAsync(DamageSeverity severity);
    Task<IEnumerable<VehicleDamage>> GetDamagesByStatusAsync(DamageStatus status);
    Task<IEnumerable<VehicleDamage>> GetUnresolvedDamagesAsync();
    Task<VehicleDamage?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<VehicleDamage>> GetAllWithDetailsAsync();
}
