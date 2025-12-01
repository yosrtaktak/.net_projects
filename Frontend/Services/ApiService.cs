using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface IApiService
{
    // Vehicles
    Task<List<Vehicle>> GetVehiclesAsync();
    Task<Vehicle?> GetVehicleAsync(int id);
    Task<List<Vehicle>> GetAvailableVehiclesAsync(DateTime startDate, DateTime endDate);
    Task<List<Vehicle>> GetVehiclesByCategoryAsync(VehicleCategory category);
    Task<Vehicle?> CreateVehicleAsync(CreateVehicleRequest request);
    Task<bool> UpdateVehicleAsync(int id, Vehicle vehicle);
    Task<bool> DeleteVehicleAsync(int id);
    Task<VehicleHistoryResponse?> GetVehicleHistoryAsync(int id);
    
    // Customers
    Task<List<Customer>> GetCustomersAsync();
    Task<Customer?> GetCustomerAsync(int id);
    
    // Rentals
    Task<List<Rental>> GetRentalsAsync();
    Task<Rental?> GetRentalAsync(int id);
    Task<List<Rental>> GetCustomerRentalsAsync(int customerId);
    Task<Rental?> CreateRentalAsync(CreateRentalRequest request);
    Task<PriceCalculationResponse?> CalculatePriceAsync(CalculatePriceRequest request);
    Task<bool> CompleteRentalAsync(int id);
    Task<bool> CancelRentalAsync(int id);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Vehicles
    public async Task<List<Vehicle>> GetVehiclesAsync()
    {
        try
        {
            var vehicles = await _httpClient.GetFromJsonAsync<List<Vehicle>>("api/vehicles");
            return vehicles ?? new List<Vehicle>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching vehicles: {ex.Message}");
            return new List<Vehicle>();
        }
    }

    public async Task<Vehicle?> GetVehicleAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Vehicle>($"api/vehicles/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching vehicle: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Vehicle>> GetAvailableVehiclesAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var vehicles = await _httpClient.GetFromJsonAsync<List<Vehicle>>(
                $"api/vehicles/available?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            return vehicles ?? new List<Vehicle>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching available vehicles: {ex.Message}");
            return new List<Vehicle>();
        }
    }

    public async Task<List<Vehicle>> GetVehiclesByCategoryAsync(VehicleCategory category)
    {
        try
        {
            var vehicles = await _httpClient.GetFromJsonAsync<List<Vehicle>>($"api/vehicles/category/{category}");
            return vehicles ?? new List<Vehicle>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching vehicles by category: {ex.Message}");
            return new List<Vehicle>();
        }
    }

    public async Task<Vehicle?> CreateVehicleAsync(CreateVehicleRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/vehicles", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Vehicle>();
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating vehicle: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateVehicleAsync(int id, Vehicle vehicle)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/vehicles/{id}", vehicle);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating vehicle: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteVehicleAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/vehicles/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting vehicle: {ex.Message}");
            return false;
        }
    }

    public async Task<VehicleHistoryResponse?> GetVehicleHistoryAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<VehicleHistoryResponse>($"api/vehicles/{id}/history");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching vehicle history: {ex.Message}");
            return null;
        }
    }

    // Customers
    public async Task<List<Customer>> GetCustomersAsync()
    {
        try
        {
            var customers = await _httpClient.GetFromJsonAsync<List<Customer>>("api/customers");
            return customers ?? new List<Customer>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching customers: {ex.Message}");
            return new List<Customer>();
        }
    }

    public async Task<Customer?> GetCustomerAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Customer>($"api/customers/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching customer: {ex.Message}");
            return null;
        }
    }

    // Rentals
    public async Task<List<Rental>> GetRentalsAsync()
    {
        try
        {
            var rentals = await _httpClient.GetFromJsonAsync<List<Rental>>("api/rentals");
            return rentals ?? new List<Rental>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching rentals: {ex.Message}");
            return new List<Rental>();
        }
    }

    public async Task<Rental?> GetRentalAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Rental>($"api/rentals/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching rental: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Rental>> GetCustomerRentalsAsync(int customerId)
    {
        try
        {
            var rentals = await _httpClient.GetFromJsonAsync<List<Rental>>($"api/rentals/customer/{customerId}");
            return rentals ?? new List<Rental>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching customer rentals: {ex.Message}");
            return new List<Rental>();
        }
    }

    public async Task<Rental?> CreateRentalAsync(CreateRentalRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/rentals", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Rental>();
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating rental: {ex.Message}");
            return null;
        }
    }

    public async Task<PriceCalculationResponse?> CalculatePriceAsync(CalculatePriceRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/rentals/calculate-price", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PriceCalculationResponse>();
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calculating price: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CompleteRentalAsync(int id)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/rentals/{id}/complete", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error completing rental: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CancelRentalAsync(int id)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/rentals/{id}/cancel", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cancelling rental: {ex.Message}");
            return false;
        }
    }
}
