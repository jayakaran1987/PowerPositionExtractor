using PowerPositionExtractor.Logic.Model;
using Services;

namespace PowerPositionExtractor.Logic.Interfaces;

public interface ITradeAggregator
{
    List<Trade> Aggregate(IEnumerable<PowerTrade> trades);
}