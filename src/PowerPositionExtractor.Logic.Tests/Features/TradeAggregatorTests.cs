using Microsoft.Extensions.Logging;
using NSubstitute;
using PowerPositionExtractor.Logic.Features;
using PowerPositionExtractor.Logic.Model;
using Services;
using Shouldly;

namespace PowerPositionExtractor.Logic.Tests.Features;

public class TradeAggregatorTests
{
    private readonly TradeAggregator _feature;
    public TradeAggregatorTests()
    {
        var logger = Substitute.For<ILogger<TradeAggregator>>();
        _feature = new TradeAggregator(logger);
    }
    [Fact]
    public void Given_Null_Trade_When_Aggregate_Then_ThrowException()
    {
        //Given
        IEnumerable<PowerTrade> trades = null;
        //When and Then
        Should.Throw<ArgumentNullException>(() => _feature.Aggregate(trades));
    }
    
    [Theory]
    [MemberData(nameof(AggregationTestData))]
    public void Given_Trade_When_Aggregate_Then_ExpectedResult(double[] inputTrade1Volume, double[] inputTrade2Volume, double[] outputAggregatedVolume)
    {
        //Given
        var trades = new[]
        {
            BuildTrade(new DateTime(2015, 4, 1), inputTrade1Volume),
            BuildTrade(new DateTime(2015, 4, 1), inputTrade2Volume)
        };
        //When 
        var result = _feature.Aggregate(trades);
        //Then
        var aggregatedResults = outputAggregatedVolume
            .Select((volume, index) =>
            {
                var period = index + 1;
                var hour = (23 + index) % 24;
                var localTime = $"{hour:D2}:00";
                return new Trade(period, localTime, volume);
            })
            .ToList();
        result.ShouldBe(aggregatedResults);
    }

    public static IEnumerable<object[]> AggregationTestData =>
        new List<object[]>
        {
            new object[]
            {
                // Trade 1 volumes given in the Challange
                new double[24]
                {
                    100,100,100,100,100,100,100,100,100,100,100,100,
                    100,100,100,100,100,100,100,100,100,100,100,100
                },

                // Trade 2 volumes given in the Challange
                new double[24]
                {
                    50,50,50,50,50,50,50,50,50,50,50,-20,
                    -20,-20,-20,-20,-20,-20,-20,-20,-20,-20,-20,-20,
                },

                // Expected aggregated volumes
                new double[24]
                {
                    150,150,150,150,150,150,150,150,150,150,150,80,
                    80,80,80,80,80,80,80,80,80,80,80,80
                }
            }
        };
    //Build Trade due to the internal set on Periods
    private static PowerTrade BuildTrade(DateTime date, double[] volumes)
    {
        var trade = PowerTrade.Create(date, 24);
        foreach (var period in trade.Periods)
        {
            period.Volume = volumes[period.Period - 1];
        }
        return trade;
    }
}