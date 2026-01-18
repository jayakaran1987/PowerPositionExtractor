using Microsoft.Extensions.Logging;
using PowerPositionExtractor.Logic.Interfaces;
using PowerPositionExtractor.Logic.Model;
using Services;

namespace PowerPositionExtractor.Logic.Features;

public class TradeAggregator(ILogger<TradeAggregator> logger): ITradeAggregator
{
    public List<Trade> Aggregate(IEnumerable<PowerTrade> trades)
    {
        if (trades == null)
        {
            throw new ArgumentNullException(nameof(trades));
        }
        //Do the aggregation for all trades per period.
        //Key is the Period and value is the sum of the volume
        var aggregatedPeriodVolumes = trades
            .SelectMany(t => t.Periods)
            .GroupBy(p => p.Period)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.Volume));
        
        var output = new List<Trade>();
        //Logic to map the LocalTimeHour to period
        // The period number starts at 1, which is the first period of the day and starts at 23:00 (11 pm) on the previous day.
        var baseHour = 23;
        for (int period = 1; period <= 24; period++)
        {
            var hour = (baseHour + (period - 1)) % 24;
            var localTimeLabel = $"{hour:D2}:00";
            output.Add(new Trade(period, localTimeLabel, aggregatedPeriodVolumes.GetValueOrDefault(period)));
        }
        return output;
    }
}