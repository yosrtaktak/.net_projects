using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Frontend.Models;

namespace Frontend.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetUsernameAsync();
    Task<string?> GetRoleAsync();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private const string TokenKey = "authToken";
    private const string UsernameKey = "username";
    private const string RoleKey = "userRole";

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                
                if (authResponse != null)
                {
                    await _localStorage.SetItemAsync(TokenKey, authResponse.Token);
                    await _localStorage.SetItemAsync(UsernameKey, authResponse.Username);
                    await _localStorage.SetItemAsync(RoleKey, authResponse.Role);
                    
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", authResponse.Token);
                }
                
                return authResponse;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                
                if (authResponse != null)
                {
                    await _localStorage.SetItemAsync(TokenKey, authResponse.Token);
                    await _localStorage.SetItemAsync(UsernameKey, authResponse.Username);
                    await _localStorage.SetItemAsync(RoleKey, authResponse.Role);
                    
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", authResponse.Token);
                }
                
                return authResponse;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Register error: {ex.Message}");
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync(UsernameKey);
        await _localStorage.RemoveItemAsync(RoleKey);
        
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>(TokenKey);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetUsernameAsync()
    {
        return await _localStorage.GetItemAsync<string>(UsernameKey);
    }

    public async Task<string?> GetRoleAsync()
    {
        return await _localStorage.GetItemAsync<string>(RoleKey);
    }
}
