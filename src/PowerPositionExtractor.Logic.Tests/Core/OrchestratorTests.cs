using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using PowerPositionExtractor.Logic.Core;
using PowerPositionExtractor.Logic.Features;
using PowerPositionExtractor.Logic.Interfaces;
using PowerPositionExtractor.Logic.Model;
using PowerPositionExtractor.Logic.Model.Options;
using Services;
using Shouldly;

namespace PowerPositionExtractor.Logic.Tests.Core;

public class OrchestratorTests
{
    private readonly ILogger<Orchestrator> _logger;
    private readonly ITradeAggregator _tradeAggregator;
    private readonly ICsvBuilder _csvBuilder;
    private readonly IPowerService _powerService;
    private readonly IStorageStrategy _storageStrategy;
    private readonly IOptions<ExtractorOptions> _extractorOptions;

    public OrchestratorTests()
    {
        _logger = Substitute.For<ILogger<Orchestrator>>();
        _tradeAggregator = Substitute.For<ITradeAggregator>();
        _csvBuilder = Substitute.For<ICsvBuilder>();
        _powerService = Substitute.For<IPowerService>();
        _storageStrategy = Substitute.For<IStorageStrategy>();
        _extractorOptions = Substitute.For<IOptions<ExtractorOptions>>();
    }

    [Fact]
    public void Given_Null_ExtractorOptions_When_RunOrchestratorAsync_Then_ArgumentException()
    {
        //Given
        _extractorOptions.Value.Returns(new ExtractorOptions { StoragePath = null });
        //When and Then
        var orchestrator = new Orchestrator(_logger, _tradeAggregator, _csvBuilder, _powerService, _storageStrategy, _extractorOptions);
        Should.Throw<ArgumentException>(() =>  orchestrator.RunOrchestratorAsync(CancellationToken.None));
    }
    
    [Fact]
    public async Task Given_Valid_Configuration_When_RunOrchestratorAsync_Then_Full_Flow_Works()
    {
        //Given
        _extractorOptions.Value.Returns(new ExtractorOptions { StoragePath = "somepath" });
        var trades = new List<PowerTrade>();
        var aggregatedTrades = new List<Trade>();

        _powerService.GetTradesAsync(Arg.Any<DateTime>())
            .Returns(trades);
        _tradeAggregator.Aggregate(trades)
            .Returns(aggregatedTrades);
        _csvBuilder.BuildCsvContent(aggregatedTrades)
            .Returns("csv-content");
        //When
        var orchestrator = new Orchestrator(_logger, _tradeAggregator, _csvBuilder, _powerService, _storageStrategy, _extractorOptions);
        await orchestrator.RunOrchestratorAsync(CancellationToken.None);
        //Then
        await _powerService.Received(1) .GetTradesAsync(Arg.Any<DateTime>());
        _tradeAggregator.Received(1) .Aggregate(trades);
        _csvBuilder.Received(1).BuildCsvContent(aggregatedTrades);
        await _storageStrategy.Received(1) .WriteReportAsync(
                Arg.Is<Report>(r=> r.StoragePath == "somepath" && r.Content == "csv-content"), Arg.Any<CancellationToken>());
    }
}