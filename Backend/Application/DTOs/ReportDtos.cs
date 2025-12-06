namespace Backend.Application.DTOs;

public class DashboardReportDto
{
    public int TotalVehicles { get; set; }
    public int AvailableVehicles { get; set; }
    public int RentedVehicles { get; set; }
    public int UnderMaintenanceVehicles { get; set; }
    public int TotalRentals { get; set; }
    public int ActiveRentals { get; set; }
    public int ReservedRentals { get; set; }
    public int CompletedRentals { get; set; }
    public int CancelledRentals { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public List<VehicleCategoryReportDto> VehiclesByCategory { get; set; } = new();
    public List<MonthlyRevenueDto> RevenueByMonth { get; set; } = new();
    public List<TopVehicleDto> TopRentedVehicles { get; set; } = new();
    public List<RecentRentalDto> RecentRentals { get; set; } = new();
}

public class VehicleCategoryReportDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Available { get; set; }
    public int Rented { get; set; }
}

public class MonthlyRevenueDto
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Revenue { get; set; }
    public int RentalCount { get; set; }
}

public class TopVehicleDto
{
    public int VehicleId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int RentalCount { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class RecentRentalDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string VehicleName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
}

public class RentalStatisticsDto
{
    public int TotalRentals { get; set; }
    public int ActiveRentals { get; set; }
    public int CompletedRentals { get; set; }
    public int CancelledRentals { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRentalCost { get; set; }
    public int AverageRentalDuration { get; set; }
}

public class VehicleUtilizationDto
{
    public int VehicleId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int TotalRentals { get; set; }
    public int TotalDaysRented { get; set; }
    public decimal UtilizationRate { get; set; }
    public decimal TotalRevenue { get; set; }
}
