using Backend.Core.Entities;

namespace Backend.Core.Interfaces;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Vehicle>> GetVehiclesByCategoryAsync(VehicleCategory category);
    Task<IEnumerable<Vehicle>> GetVehiclesByStatusAsync(VehicleStatus status);
    Task<Vehicle?> GetVehicleWithRentalsAsync(int vehicleId);
    Task<Vehicle?> GetByIdWithHistoryAsync(int vehicleId);
}
