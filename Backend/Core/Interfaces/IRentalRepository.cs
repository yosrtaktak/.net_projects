using Backend.Core.Entities;

namespace Backend.Core.Interfaces;

public interface IRentalRepository : IRepository<Rental>
{
    Task<IEnumerable<Rental>> GetActiveRentalsAsync();
    Task<IEnumerable<Rental>> GetRentalsByUserIdAsync(string userId);
    Task<IEnumerable<Rental>> GetRentalsByVehicleAsync(int vehicleId);
    Task<Rental?> GetByIdWithDetailsAsync(int rentalId);
    Task<IEnumerable<Rental>> GetAllWithDetailsAsync();
    Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Rental>> GetRentalsForManagementAsync(string? status, DateTime? startDate, DateTime? endDate, int? vehicleId, string? userId);
}
