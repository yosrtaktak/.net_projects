using Backend.Core.Entities;
using Backend.Core.Interfaces;

namespace Backend.Application.Services.PricingStrategies;

public class SeasonalPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Seasonal Pricing";

    public decimal CalculatePrice(Vehicle vehicle, DateTime startDate, DateTime endDate, ApplicationUser user)
    {
        var days = (endDate - startDate).Days;
        if (days < 1) days = 1;
        
        var basePrice = vehicle.DailyRate * days;
        
        // High season: June-August (Summer) and December (Holidays)
        var isHighSeason = (startDate.Month >= 6 && startDate.Month <= 8) || startDate.Month == 12;
        
        if (isHighSeason)
        {
            basePrice *= 1.25m; // 25% increase during high season
        }
        
        return basePrice;
    }
}
