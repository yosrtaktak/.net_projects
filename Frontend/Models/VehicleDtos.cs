using System.Text.Json.Serialization;

namespace Frontend.Models;

public enum VehicleStatus
{
    Available,      // Disponible - le véhicule est disponible à la location
    Reserved,       // Réservé - le véhicule est réservé mais pas encore récupéré
    Rented,         // Loué actuellement - le véhicule est en cours de location
    Maintenance,    // En maintenance - le véhicule est en réparation/entretien
    Retired         // Hors service - le véhicule n'est plus utilisable
}

public class Vehicle
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("brand")]
    public string Brand { get; set; } = string.Empty;
    
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("registrationNumber")]
    public string RegistrationNumber { get; set; } = string.Empty;

    // Alias property for API compatibility
    [JsonIgnore]
    public string LicensePlate
    {
        get => RegistrationNumber;
        set => RegistrationNumber = value;
    }

    [JsonPropertyName("year")]
    public int Year { get; set; }
    
    // Foreign key to Category
    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }
    
    // Navigation property
    [JsonPropertyName("category")]
    public CategoryModel? Category { get; set; }
    
    // Helper property for backward compatibility
    [JsonIgnore]
    public string CategoryName => Category?.Name ?? "Unknown";
    
    [JsonPropertyName("dailyRate")]
    public decimal DailyRate { get; set; }
    
    [JsonPropertyName("status")]
    public VehicleStatus Status { get; set; }
    
    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }
    
    [JsonPropertyName("mileage")]
    public int Mileage { get; set; }
    
    [JsonPropertyName("fuelType")]
    public string? FuelType { get; set; }
    
    [JsonPropertyName("seatingCapacity")]
    public int SeatingCapacity { get; set; }
}

public class CreateVehicleRequest
{
    [JsonPropertyName("brand")]
    public string Brand { get; set; } = string.Empty;
    
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("registrationNumber")]
    public string RegistrationNumber { get; set; } = string.Empty;
    
    [JsonPropertyName("year")]
    public int Year { get; set; }
    
    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }
    
    [JsonPropertyName("dailyRate")]
    public decimal DailyRate { get; set; }
    
    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }
    
    [JsonPropertyName("mileage")]
    public int Mileage { get; set; }
    
    [JsonPropertyName("fuelType")]
    public string? FuelType { get; set; }
    
    [JsonPropertyName("seatingCapacity")]
    public int SeatingCapacity { get; set; }
}
