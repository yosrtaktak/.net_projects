using Backend.Core.Entities;

namespace Backend.Core.Interfaces;

public interface IPricingStrategy
{
    decimal CalculatePrice(Vehicle vehicle, DateTime startDate, DateTime endDate, ApplicationUser user);
    string StrategyName { get; }
}
