using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Core.Entities;

public class Vehicle
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string RegistrationNumber { get; set; } = string.Empty;
    
    public int Year { get; set; }
    
    // Foreign key to Category table
    public int CategoryId { get; set; }
    
    // Navigation property
    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;
    
    public decimal DailyRate { get; set; }
    
    public VehicleStatus Status { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public int Mileage { get; set; }
    
    public string? FuelType { get; set; }
    
    public int SeatingCapacity { get; set; }
    
    // Navigation properties
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public ICollection<Maintenance> MaintenanceRecords { get; set; } = new List<Maintenance>();
    public ICollection<VehicleDamage> DamageRecords { get; set; } = new List<VehicleDamage>();
}

public enum VehicleStatus
{
    Available,      // Disponible - le véhicule est disponible à la location
    Reserved,       // Réservé - le véhicule est réservé mais pas encore récupéré
    Rented,         // Loué actuellement - le véhicule est en cours de location
    Maintenance,    // En maintenance - le véhicule est en réparation/entretien
    Retired         // Hors service - le véhicule n'est plus utilisable
}
