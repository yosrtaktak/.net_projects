using Backend.Core.Interfaces;

namespace Backend.Application.Factories;

public interface IPricingStrategyFactory
{
    IPricingStrategy CreateStrategy(string strategyType);
    IEnumerable<string> GetAvailableStrategies();
}
