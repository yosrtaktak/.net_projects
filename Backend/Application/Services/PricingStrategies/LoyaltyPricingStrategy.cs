using Backend.Core.Entities;
using Backend.Core.Interfaces;

namespace Backend.Application.Services.PricingStrategies;

public class LoyaltyPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Loyalty Pricing";

    public decimal CalculatePrice(Vehicle vehicle, DateTime startDate, DateTime endDate, ApplicationUser user)
    {
        var days = (endDate - startDate).Days;
        if (days < 1) days = 1;
        
        var basePrice = vehicle.DailyRate * days;
        
        // Apply discount based on customer tier
        var discount = user.Tier switch
        {
            CustomerTier.Silver => 0.05m,     // 5% discount
            CustomerTier.Gold => 0.10m,       // 10% discount
            CustomerTier.Platinum => 0.15m,   // 15% discount
            _ => 0m
        };
        
        return basePrice * (1 - discount);
    }
}
