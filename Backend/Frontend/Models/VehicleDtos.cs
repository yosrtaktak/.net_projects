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
    public int Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;

    // Alias property for API compatibility
    public string LicensePlate
    {
        get => RegistrationNumber;
        set => RegistrationNumber = value;
    }

    public int Year { get; set; }
    
    // Foreign key to Category
    public int CategoryId { get; set; }
    
    // Navigation property
    public CategoryModel? Category { get; set; }
    
    // Helper property for backward compatibility
    public string CategoryName => Category?.Name ?? "Unknown";
    
    public decimal DailyRate { get; set; }
    public VehicleStatus Status { get; set; }
    public string? ImageUrl { get; set; }
    public int Mileage { get; set; }
    public string? FuelType { get; set; }
    public int SeatingCapacity { get; set; }
}

public class CreateVehicleRequest
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public int Year { get; set; }
    public int CategoryId { get; set; }
    public decimal DailyRate { get; set; }
    public string? ImageUrl { get; set; }
    public int Mileage { get; set; }
    public string? FuelType { get; set; }
    public int SeatingCapacity { get; set; }
}
