using Backend.Core.Entities;
using Backend.Core.Interfaces;

namespace Backend.Application.Services.PricingStrategies;

public class StandardPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Standard Pricing";

    public decimal CalculatePrice(Vehicle vehicle, DateTime startDate, DateTime endDate, Customer customer)
    {
        var days = (endDate - startDate).Days;
        if (days < 1) days = 1;
        
        return vehicle.DailyRate * days;
    }
}
