using Backend.Core.Entities;

namespace Backend.Core.Interfaces;

public interface IPricingStrategy
{
    decimal CalculatePrice(Vehicle vehicle, DateTime startDate, DateTime endDate, Customer customer);
    string StrategyName { get; }
}
