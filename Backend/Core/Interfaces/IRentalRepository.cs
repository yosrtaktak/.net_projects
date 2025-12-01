using Backend.Core.Entities;

namespace Backend.Core.Interfaces;

public interface IRentalRepository : IRepository<Rental>
{
    Task<IEnumerable<Rental>> GetActiveRentalsAsync();
    Task<IEnumerable<Rental>> GetRentalsByCustomerAsync(int customerId);
    Task<IEnumerable<Rental>> GetRentalsByVehicleAsync(int vehicleId);
    Task<Rental?> GetRentalWithDetailsAsync(int rentalId);
    Task<Rental?> GetRentalWithCustomerAsync(int rentalId);
    Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startDate, DateTime endDate);
}
