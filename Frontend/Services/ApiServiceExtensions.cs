using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services;

// Extension methods for ApiService to add maintenance and damage operations
public static class ApiServiceExtensions
{
    // Maintenance methods
    public static async Task<List<MaintenanceDto>> GetMaintenancesAsync(this IApiService apiService, HttpClient httpClient, MaintenanceFilterDto? filter = null)
    {
        try
        {
            var query = filter != null ? BuildMaintenanceQueryString(filter) : "";
            var maintenances = await httpClient.GetFromJsonAsync<List<MaintenanceDto>>($"api/maintenances{query}");
            return maintenances ?? new List<MaintenanceDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching maintenances: {ex.Message}");
            return new List<MaintenanceDto>();
        }
    }

    public static async Task<MaintenanceDto?> GetMaintenanceAsync(this IApiService apiService, HttpClient httpClient, int id)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<MaintenanceDto>($"api/maintenances/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching maintenance: {ex.Message}");
            return null;
        }
    }

    public static async Task<MaintenanceDto?> CreateMaintenanceAsync(this IApiService apiService, HttpClient httpClient, CreateMaintenanceDto request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/maintenances", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MaintenanceDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating maintenance: {ex.Message}");
            return null;
        }
    }

    public static async Task<bool> CompleteMaintenanceAsync(this IApiService apiService, HttpClient httpClient, int id, CompleteMaintenanceDto request)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"api/maintenances/{id}/complete", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error completing maintenance: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> CancelMaintenanceAsync(this IApiService apiService, HttpClient httpClient, int id)
    {
        try
        {
            var response = await httpClient.PutAsync($"api/maintenances/{id}/cancel", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cancelling maintenance: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> DeleteMaintenanceAsync(this IApiService apiService, HttpClient httpClient, int id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"api/maintenances/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting maintenance: {ex.Message}");
            return false;
        }
    }

    // Vehicle Damage methods
    public static async Task<List<VehicleDamageDto>> GetVehicleDamagesAsync(this IApiService apiService, HttpClient httpClient, DamageFilterDto? filter = null)
    {
        try
        {
            var query = filter != null ? BuildDamageQueryString(filter) : "";
            var damages = await httpClient.GetFromJsonAsync<List<VehicleDamageDto>>($"api/vehicledamages{query}");
            return damages ?? new List<VehicleDamageDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching vehicle damages: {ex.Message}");
            return new List<VehicleDamageDto>();
        }
    }

    public static async Task<VehicleDamageDto?> GetVehicleDamageAsync(this IApiService apiService, HttpClient httpClient, int id)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<VehicleDamageDto>($"api/vehicledamages/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching vehicle damage: {ex.Message}");
            return null;
        }
    }

    public static async Task<VehicleDamageDto?> CreateVehicleDamageAsync(this IApiService apiService, HttpClient httpClient, CreateVehicleDamageDto request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/vehicledamages", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VehicleDamageDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating vehicle damage: {ex.Message}");
            return null;
        }
    }

    public static async Task<bool> StartRepairAsync(this IApiService apiService, HttpClient httpClient, int id)
    {
        try
        {
            var response = await httpClient.PutAsync($"api/vehicledamages/{id}/start-repair", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting repair: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> CompleteRepairAsync(this IApiService apiService, HttpClient httpClient, int id, RepairDamageDto request)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"api/vehicledamages/{id}/repair", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error completing repair: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> DeleteVehicleDamageAsync(this IApiService apiService, HttpClient httpClient, int id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"api/vehicledamages/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting vehicle damage: {ex.Message}");
            return false;
        }
    }

    // Helper methods
    private static string BuildMaintenanceQueryString(MaintenanceFilterDto filter)
    {
        var parameters = new List<string>();
        
        if (filter.VehicleId.HasValue)
            parameters.Add($"vehicleId={filter.VehicleId.Value}");
        if (filter.Type.HasValue)
            parameters.Add($"type={filter.Type.Value}");
        if (filter.Status.HasValue)
            parameters.Add($"status={filter.Status.Value}");
        if (filter.StartDate.HasValue)
            parameters.Add($"startDate={filter.StartDate.Value:yyyy-MM-dd}");
        if (filter.EndDate.HasValue)
            parameters.Add($"endDate={filter.EndDate.Value:yyyy-MM-dd}");
        if (filter.IsOverdue.HasValue)
            parameters.Add($"isOverdue={filter.IsOverdue.Value}");
        
        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : "";
    }

    private static string BuildDamageQueryString(DamageFilterDto filter)
    {
        var parameters = new List<string>();
        
        if (filter.VehicleId.HasValue)
            parameters.Add($"vehicleId={filter.VehicleId.Value}");
        if (filter.RentalId.HasValue)
            parameters.Add($"rentalId={filter.RentalId.Value}");
        if (filter.Severity.HasValue)
            parameters.Add($"severity={filter.Severity.Value}");
        if (filter.Status.HasValue)
            parameters.Add($"status={filter.Status.Value}");
        if (filter.StartDate.HasValue)
            parameters.Add($"startDate={filter.StartDate.Value:yyyy-MM-dd}");
        if (filter.EndDate.HasValue)
            parameters.Add($"endDate={filter.EndDate.Value:yyyy-MM-dd}");
        if (filter.UnresolvedOnly.HasValue)
            parameters.Add($"unresolvedOnly={filter.UnresolvedOnly.Value}");
        
        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : "";
    }
}
