using Backend.Core.Interfaces;
using Backend.Application.Services.PricingStrategies;

namespace Backend.Application.Factories;

public class PricingStrategyFactory : IPricingStrategyFactory
{
    private readonly Dictionary<string, Func<IPricingStrategy>> _strategies;

    public PricingStrategyFactory()
    {
        _strategies = new Dictionary<string, Func<IPricingStrategy>>(StringComparer.OrdinalIgnoreCase)
        {
            { "standard", () => new StandardPricingStrategy() },
            { "loyalty", () => new LoyaltyPricingStrategy() },
            { "seasonal", () => new SeasonalPricingStrategy() },
            { "weekend", () => new WeekendPricingStrategy() }
        };
    }

    public IPricingStrategy CreateStrategy(string strategyType)
    {
        if (_strategies.TryGetValue(strategyType, out var factory))
        {
            return factory();
        }
        
        // Default to standard pricing
        return new StandardPricingStrategy();
    }

    public IEnumerable<string> GetAvailableStrategies()
    {
        return _strategies.Keys;
    }
}
