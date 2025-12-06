using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Backend.Application.Services;

public interface IReportService
{
    Task<DashboardReportDto> GetDashboardReportAsync();
    Task<RentalStatisticsDto> GetRentalStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<VehicleUtilizationDto>> GetVehicleUtilizationReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<MonthlyRevenueDto>> GetMonthlyRevenueReportAsync(int months = 12);
}

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReportService(
        IUnitOfWork unitOfWork,
        IVehicleRepository vehicleRepository,
        IRentalRepository rentalRepository,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _vehicleRepository = vehicleRepository;
        _rentalRepository = rentalRepository;
        _userManager = userManager;
    }

    public async Task<DashboardReportDto> GetDashboardReportAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        var rentals = await _rentalRepository.GetAllAsync();
        
        // Get count of users with Customer role
        var allUsers = _userManager.Users.ToList();
        var customerCount = 0;
        foreach (var user in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Customer"))
            {
                customerCount++;
            }
        }

        var vehiclesList = vehicles.ToList();
        var rentalsList = rentals.ToList();

        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var report = new DashboardReportDto
        {
            TotalVehicles = vehiclesList.Count,
            AvailableVehicles = vehiclesList.Count(v => v.Status == VehicleStatus.Available),
            RentedVehicles = vehiclesList.Count(v => v.Status == VehicleStatus.Rented),
            UnderMaintenanceVehicles = vehiclesList.Count(v => v.Status == VehicleStatus.Maintenance),
            
            TotalRentals = rentalsList.Count,
            ActiveRentals = rentalsList.Count(r => r.Status == RentalStatus.Active),
            ReservedRentals = rentalsList.Count(r => r.Status == RentalStatus.Reserved),
            CompletedRentals = rentalsList.Count(r => r.Status == RentalStatus.Completed),
            CancelledRentals = rentalsList.Count(r => r.Status == RentalStatus.Cancelled),
            
            TotalCustomers = customerCount,
            TotalRevenue = rentalsList
                .Where(r => r.Status == RentalStatus.Completed)
                .Sum(r => r.TotalCost),
            MonthlyRevenue = rentalsList
                .Where(r => r.Status == RentalStatus.Completed && 
                           r.StartDate.Month == currentMonth && 
                           r.StartDate.Year == currentYear)
                .Sum(r => r.TotalCost)
        };

        // Vehicles by category
        report.VehiclesByCategory = vehiclesList
            .GroupBy(v => v.Category)
            .Select(g => new VehicleCategoryReportDto
            {
                Category = g.Key.ToString(),
                Count = g.Count(),
                Available = g.Count(v => v.Status == VehicleStatus.Available),
                Rented = g.Count(v => v.Status == VehicleStatus.Rented)
            })
            .ToList();

        // Revenue by month (last 6 months)
        report.RevenueByMonth = rentalsList
            .Where(r => r.Status == RentalStatus.Completed && r.StartDate >= DateTime.UtcNow.AddMonths(-6))
            .GroupBy(r => new { r.StartDate.Year, r.StartDate.Month })
            .Select(g => new MonthlyRevenueDto
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                Year = g.Key.Year,
                Revenue = g.Sum(r => r.TotalCost),
                RentalCount = g.Count()
            })
            .OrderBy(m => m.Year)
            .ThenBy(m => DateTime.Parse(m.Month).Month)
            .ToList();

        // Top rented vehicles
        report.TopRentedVehicles = rentalsList
            .Where(r => r.Status == RentalStatus.Completed)
            .GroupBy(r => new { r.VehicleId, r.Vehicle!.Brand, r.Vehicle.Model })
            .Select(g => new TopVehicleDto
            {
                VehicleId = g.Key.VehicleId,
                Brand = g.Key.Brand,
                Model = g.Key.Model,
                RentalCount = g.Count(),
                TotalRevenue = g.Sum(r => r.TotalCost)
            })
            .OrderByDescending(t => t.RentalCount)
            .Take(5)
            .ToList();

        // Recent rentals - load with user details
        var recentRentalsQuery = await _rentalRepository.GetAllWithDetailsAsync();
        report.RecentRentals = recentRentalsQuery
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .Select(r => new RecentRentalDto
            {
                Id = r.Id,
                CustomerName = r.User != null ? $"{r.User.FirstName} {r.User.LastName}" : "N/A",
                VehicleName = r.Vehicle != null ? $"{r.Vehicle.Brand} {r.Vehicle.Model}" : "N/A",
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Status = r.Status.ToString(),
                TotalCost = r.TotalCost
            })
            .ToList();

        return report;
    }

    public async Task<RentalStatisticsDto> GetRentalStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var rentals = await _rentalRepository.GetAllAsync();
        var rentalsList = rentals.ToList();

        if (startDate.HasValue)
        {
            rentalsList = rentalsList.Where(r => r.StartDate >= startDate.Value).ToList();
        }

        if (endDate.HasValue)
        {
            rentalsList = rentalsList.Where(r => r.EndDate <= endDate.Value).ToList();
        }

        var completedRentals = rentalsList.Where(r => r.Status == RentalStatus.Completed).ToList();

        return new RentalStatisticsDto
        {
            TotalRentals = rentalsList.Count,
            ActiveRentals = rentalsList.Count(r => r.Status == RentalStatus.Active),
            CompletedRentals = completedRentals.Count,
            CancelledRentals = rentalsList.Count(r => r.Status == RentalStatus.Cancelled),
            TotalRevenue = completedRentals.Sum(r => r.TotalCost),
            AverageRentalCost = completedRentals.Any() ? completedRentals.Average(r => r.TotalCost) : 0,
            AverageRentalDuration = completedRentals.Any() 
                ? (int)completedRentals.Average(r => (r.EndDate - r.StartDate).Days) 
                : 0
        };
    }

    public async Task<List<VehicleUtilizationDto>> GetVehicleUtilizationReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        var rentals = await _rentalRepository.GetAllAsync();
        
        var rentalsList = rentals.ToList();
        
        if (startDate.HasValue)
        {
            rentalsList = rentalsList.Where(r => r.StartDate >= startDate.Value).ToList();
        }

        if (endDate.HasValue)
        {
            rentalsList = rentalsList.Where(r => r.EndDate <= endDate.Value).ToList();
        }

        var reportPeriodDays = (endDate ?? DateTime.UtcNow) - (startDate ?? DateTime.UtcNow.AddMonths(-12));
        var totalDays = Math.Max(1, reportPeriodDays.Days);

        return vehicles.Select(v =>
        {
            var vehicleRentals = rentalsList
                .Where(r => r.VehicleId == v.Id && r.Status == RentalStatus.Completed)
                .ToList();

            var totalDaysRented = vehicleRentals.Sum(r => (r.EndDate - r.StartDate).Days);

            return new VehicleUtilizationDto
            {
                VehicleId = v.Id,
                Brand = v.Brand,
                Model = v.Model,
                TotalRentals = vehicleRentals.Count,
                TotalDaysRented = totalDaysRented,
                UtilizationRate = (decimal)totalDaysRented / totalDays * 100,
                TotalRevenue = vehicleRentals.Sum(r => r.TotalCost)
            };
        })
        .OrderByDescending(u => u.UtilizationRate)
        .ToList();
    }

    public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueReportAsync(int months = 12)
    {
        var startDate = DateTime.UtcNow.AddMonths(-months);
        var rentals = await _rentalRepository.GetAllAsync();

        return rentals
            .Where(r => r.Status == RentalStatus.Completed && r.StartDate >= startDate)
            .GroupBy(r => new { r.StartDate.Year, r.StartDate.Month })
            .Select(g => new MonthlyRevenueDto
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                Year = g.Key.Year,
                Revenue = g.Sum(r => r.TotalCost),
                RentalCount = g.Count()
            })
            .OrderBy(m => m.Year)
            .ThenBy(m => DateTime.Parse(m.Month).Month)
            .ToList();
    }
}
