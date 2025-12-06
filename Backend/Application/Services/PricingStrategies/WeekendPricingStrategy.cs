using Backend.Core.Entities;
using Backend.Core.Interfaces;

namespace Backend.Application.Services.PricingStrategies;

public class WeekendPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Weekend Pricing";

    public decimal CalculatePrice(Vehicle vehicle, DateTime startDate, DateTime endDate, ApplicationUser user)
    {
        var days = (endDate - startDate).Days;
        if (days < 1) days = 1;
        
        var basePrice = vehicle.DailyRate * days;
        
        // Count weekend days
        var weekendDays = 0;
        for (var date = startDate; date < endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                weekendDays++;
            }
        }
        
        // 15% surcharge for weekend days
        var weekendSurcharge = vehicle.DailyRate * weekendDays * 0.15m;
        
        return basePrice + weekendSurcharge;
    }
}
