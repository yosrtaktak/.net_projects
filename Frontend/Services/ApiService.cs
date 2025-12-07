using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface IApiService
{
    // Vehicles
    Task<List<Vehicle>> GetVehiclesAsync();
    Task<Vehicle?> GetVehicleAsync(int id);
    Task<List<Vehicle>> GetAvailableVehiclesAsync(DateTime startDate, DateTime endDate);
    Task<List<Vehicle>> GetVehiclesByCategoryIdAsync(int categoryId);
    Task<Vehicle?> CreateVehicleAsync(CreateVehicleRequest request);
    Task<bool> UpdateVehicleAsync(int id, Vehicle vehicle);
    Task<bool> DeleteVehicleAsync(int id);
    Task<VehicleHistoryResponse?> GetVehicleHistoryAsync(int id);
    
    // Customers / Users
    Task<List<Customer>> GetCustomersAsync();
    Task<Customer?> GetCustomerAsync(int id);
    Task<Customer?> GetUserByIdAsync(string id);
    Task<Customer?> GetCurrentCustomerAsync(); // Uses /api/users/me
    Task<Customer?> GetMyProfileAsync();
    Task<UserProfileDto?> GetMyUserProfileAsync(); // For Admin/Employee profile
    Task<List<Rental>> GetMyRentalsAsync();
    Task<List<VehicleDamage>> GetMyDamagesAsync();
    Task<bool> UpdateCustomerAsync(string id, UpdateCustomerModel model);
    Task<bool> UpdateProfileAsync(UpdateProfileModel model);
    Task<bool> UpdateMyUserProfileAsync(UpdateProfileDto model); // For Admin/Employee profile update
    Task<bool> UpdateMyProfileAsync(UpdateProfileDto model); // For Customer profile update
    
    // Employee Management
    Task<List<EmployeeModel>> GetEmployeesAsync();
    Task<EmployeeModel?> CreateEmployeeAsync(CreateEmployeeModel model);
    Task<bool> UpdateUserRoleAsync(string id, string role);
    Task<bool> DeleteUserAsync(string id);

    // Rentals
    Task<List<Rental>> GetRentalsAsync();
    Task<Rental?> GetRentalAsync(int id);
    Task<List<Rental>> GetCustomerRentalsAsync(int customerId);
    Task<Rental?> CreateRentalAsync(CreateRentalRequest request);
    Task<PriceCalculationResponse?> CalculatePriceAsync(CalculatePriceRequest request);
    Task<bool> CompleteRentalAsync(int id, int endMileage);
    Task<bool> CancelRentalAsync(int id);
    Task<List<Rental>> GetRentalsForManagementAsync(string? status = null, DateTime? startDate = null, DateTime? endDate = null, int? vehicleId = null, string? userId = null);
    Task<bool> UpdateRentalStatusAsync(int id, string status);

    // Vehicle Damages
    Task<List<VehicleDamage>> GetVehicleDamagesAsync();
    Task<VehicleDamage?> GetVehicleDamageAsync(int id);
    Task<List<VehicleDamage>> GetDamagesByRentalAsync(int rentalId);
    Task<VehicleDamage?> CreateVehicleDamageAsync(CreateVehicleDamageRequest request);

    // Reports
    Task<DashboardReport?> GetDashboardReportAsync();
    Task<RentalStatistics?> GetRentalStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<VehicleUtilization>> GetVehicleUtilizationReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<MonthlyRevenue>> GetMonthlyRevenueReportAsync(int months = 12);
    
    // Categories
    Task<List<CategoryModel>> GetCategoriesAsync(bool activeOnly = false);
    Task<CategoryModel?> GetCategoryAsync(int id);
    Task<CategoryModel?> CreateCategoryAsync(CreateCategoryModel model);
    Task<CategoryModel?> UpdateCategoryAsync(int id, UpdateCategoryModel model);
    Task<bool> DeleteCategoryAsync(int id);
    Task<CategoryModel?> ToggleCategoryActiveAsync(int id);
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

    public async Task<List<Vehicle>> GetVehiclesByCategoryIdAsync(int categoryId)
    {
        try
        {
            var vehicles = await _httpClient.GetFromJsonAsync<List<Vehicle>>($"api/vehicles/category/{categoryId}");
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
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error deleting vehicle. Status: {response.StatusCode}, Content: {errorContent}");
                
                // Try to parse error message for better error handling
                try
                {
                    var errorObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                    if (errorObj != null && errorObj.ContainsKey("message"))
                    {
                        throw new InvalidOperationException(errorObj["message"].ToString());
                    }
                }
                catch (System.Text.Json.JsonException)
                {
                    // If parsing fails, throw generic error
                    throw new InvalidOperationException("Failed to delete vehicle. It may have active rentals or maintenance records.");
                }
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting vehicle: {ex.Message}");
            throw; // Re-throw to allow caller to handle it
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

    // Customers - Updated to use /api/users endpoints
    public async Task<List<Customer>> GetCustomersAsync()
    {
        try
        {
            var customers = await _httpClient.GetFromJsonAsync<List<Customer>>("api/users/customers");
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
            // Note: This endpoint may need backend support for int id lookup
            // For now, keeping for backward compatibility
            return await _httpClient.GetFromJsonAsync<Customer>($"api/customers/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching customer: {ex.Message}");
            return null;
        }
    }

    public async Task<Customer?> GetUserByIdAsync(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Customer>($"api/users/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching user by id: {ex.Message}");
            return null;
        }
    }

    public async Task<Customer?> GetCurrentCustomerAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Customer>("api/users/me");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching current customer: {ex.Message}");
            return null;
        }
    }

    public async Task<Customer?> GetMyProfileAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Customer>("api/users/me");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching my profile: {ex.Message}");
            return null;
        }
    }

    public async Task<UserProfileDto?> GetMyUserProfileAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserProfileDto>("api/users/me");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching my user profile: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateCustomerAsync(string id, UpdateCustomerModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/users/{id}/customer", model);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating customer: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateProfileAsync(UpdateProfileModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/users/me", model);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating profile: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateMyUserProfileAsync(UpdateProfileDto model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/users/me", model);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user profile: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateMyProfileAsync(UpdateProfileDto model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/users/me", model);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating my profile: {ex.Message}");
            return false;
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

    public async Task<bool> CompleteRentalAsync(int id, int endMileage)
    {
        try
        {
            var dto = new CompleteRentalDto { EndMileage = endMileage };
            var response = await _httpClient.PutAsJsonAsync($"api/rentals/{id}/complete", dto);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error completing rental. Status: {response.StatusCode}, Content: {errorContent}");
                
                // Try to parse and throw meaningful error
                try
                {
                    var errorObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                    if (errorObj != null && errorObj.ContainsKey("message"))
                    {
                        throw new InvalidOperationException(errorObj["message"].ToString());
                    }
                }
                catch (System.Text.Json.JsonException)
                {
                    // If parsing fails, throw generic error
                }
                
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw to preserve the error message
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error completing rental: {ex.Message}");
            throw;
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

    public async Task<List<Rental>> GetRentalsForManagementAsync(
        string? status = null, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        int? vehicleId = null, 
        string? userId = null)
    {
        try
        {
            var queryParams = new List<string>();
            
            if (!string.IsNullOrEmpty(status))
                queryParams.Add($"status={status}");
            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            if (vehicleId.HasValue)
                queryParams.Add($"vehicleId={vehicleId.Value}");
            if (!string.IsNullOrEmpty(userId))
                queryParams.Add($"userId={userId}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var rentals = await _httpClient.GetFromJsonAsync<List<Rental>>($"api/rentals/manage{queryString}");
            return rentals ?? new List<Rental>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching rentals for management: {ex.Message}");
            return new List<Rental>();
        }
    }

    public async Task<bool> UpdateRentalStatusAsync(int id, string status)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/rentals/{id}/status", new { status });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating rental status: {ex.Message}");
            return false;
        }
    }

    // Vehicle Damages
    public async Task<List<VehicleDamage>> GetVehicleDamagesAsync()
    {
        try
        {
            var damages = await _httpClient.GetFromJsonAsync<List<VehicleDamage>>("api/vehicledamages");
            return damages ?? new List<VehicleDamage>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching vehicle damages: {ex.Message}");
            return new List<VehicleDamage>();
        }
    }

    public async Task<VehicleDamage?> GetVehicleDamageAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<VehicleDamage>($"api/vehicledamages/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching vehicle damage: {ex.Message}");
            return null;
        }
    }

    public async Task<List<VehicleDamage>> GetDamagesByRentalAsync(int rentalId)
    {
        try
        {
            var damages = await _httpClient.GetFromJsonAsync<List<VehicleDamage>>($"api/vehicledamages/rental/{rentalId}");
            return damages ?? new List<VehicleDamage>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching damages for rental: {ex.Message}");
            return new List<VehicleDamage>();
        }
    }

    public async Task<VehicleDamage?> CreateVehicleDamageAsync(CreateVehicleDamageRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/vehicledamages", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VehicleDamage>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error creating vehicle damage. Status: {response.StatusCode}, Content: {errorContent}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception creating vehicle damage: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    // Reports
    public async Task<DashboardReport?> GetDashboardReportAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DashboardReport>("api/reports/dashboard");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching dashboard report: {ex.Message}");
            return null;
        }
    }

    public async Task<RentalStatistics?> GetRentalStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var queryParams = new List<string>();
            
            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            return await _httpClient.GetFromJsonAsync<RentalStatistics>($"api/reports/rentals/statistics{queryString}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching rental statistics: {ex.Message}");
            return null;
        }
    }

    public async Task<List<VehicleUtilization>> GetVehicleUtilizationReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var queryParams = new List<string>();
            
            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var utilization = await _httpClient.GetFromJsonAsync<List<VehicleUtilization>>($"api/reports/vehicles/utilization{queryString}");
            return utilization ?? new List<VehicleUtilization>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching vehicle utilization: {ex.Message}");
            return new List<VehicleUtilization>();
        }
    }

    public async Task<List<MonthlyRevenue>> GetMonthlyRevenueReportAsync(int months = 12)
    {
        try
        {
            var revenue = await _httpClient.GetFromJsonAsync<List<MonthlyRevenue>>($"api/reports/revenue/monthly?months={months}");
            return revenue ?? new List<MonthlyRevenue>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching monthly revenue: {ex.Message}");
            return new List<MonthlyRevenue>();
        }
    }

    // Categories
    public async Task<List<CategoryModel>> GetCategoriesAsync(bool activeOnly = false)
    {
        try
        {
            var url = activeOnly ? "api/categories?activeOnly=true" : "api/categories";
            var categories = await _httpClient.GetFromJsonAsync<List<CategoryModel>>(url);
            return categories ?? new List<CategoryModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching categories: {ex.Message}");
            return new List<CategoryModel>();
        }
    }

    public async Task<CategoryModel?> GetCategoryAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CategoryModel>($"api/categories/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching category: {ex.Message}");
            return null;
        }
    }

    public async Task<CategoryModel?> CreateCategoryAsync(CreateCategoryModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/categories", model);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CategoryModel>();
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating category: {ex.Message}");
            return null;
        }
    }

    public async Task<CategoryModel?> UpdateCategoryAsync(int id, UpdateCategoryModel model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/categories/{id}", model);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CategoryModel>();
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating category: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/categories/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting category: {ex.Message}");
            return false;
        }
    }

    public async Task<CategoryModel?> ToggleCategoryActiveAsync(int id)
    {
        try
        {
            var response = await _httpClient.PatchAsync($"api/categories/{id}/toggle", null);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CategoryModel>();
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error toggling category: {ex.Message}");
            return null;
        }
    }

    // Employee Management
    public async Task<List<EmployeeModel>> GetEmployeesAsync()
    {
        try
        {
            var employees = await _httpClient.GetFromJsonAsync<List<EmployeeModel>>("api/users/employees");
            return employees ?? new List<EmployeeModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching employees: {ex.Message}");
            return new List<EmployeeModel>();
        }
    }

    public async Task<EmployeeModel?> CreateEmployeeAsync(CreateEmployeeModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/employees", model);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EmployeeModel>();
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating employee: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateUserRoleAsync(string id, string role)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/users/{id}/role", new { role });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user role: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/users/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Rental>> GetMyRentalsAsync()
    {
        try
        {
            // Get current user first to get their ID
            var user = await GetCurrentCustomerAsync();
            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                Console.WriteLine("Cannot fetch rentals: User not found");
                return new List<Rental>();
            }

            var rentals = await _httpClient.GetFromJsonAsync<List<Rental>>($"api/rentals/user/{user.Id}");
            return rentals ?? new List<Rental>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching my rentals: {ex.Message}");
            return new List<Rental>();
        }
    }

    public async Task<List<VehicleDamage>> GetMyDamagesAsync()
    {
        try
        {
            var damages = await _httpClient.GetFromJsonAsync<List<VehicleDamage>>("api/vehicledamages/me");
            return damages ?? new List<VehicleDamage>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching my damages: {ex.Message}");
            return new List<VehicleDamage>();
        }
    }
}
