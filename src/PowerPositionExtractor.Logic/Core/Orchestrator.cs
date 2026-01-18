using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerPositionExtractor.Logic.Interfaces;
using PowerPositionExtractor.Logic.Model;
using PowerPositionExtractor.Logic.Model.Options;
using Services;

namespace PowerPositionExtractor.Logic.Core;

public class Orchestrator(
    ILogger<Orchestrator> logger,
    ITradeAggregator tradeAggregator,
    ICsvBuilder csvBuilder,
    IPowerService powerService,
    IStorageStrategy storageStrategy,
    IOptions<ExtractorOptions> extractorOptions): IOrchestrator
{
    private readonly ExtractorOptions _options = extractorOptions.Value;
    
    public async Task RunOrchestratorAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.StoragePath))
        {
            logger.LogError("ReportPath is not defined in configuration.");
            throw new ArgumentException("Report Path is not defined in configuration.");
        }
        //Extraction Datetime should be local Time. Local time is London Local time for the day
        TimeZoneInfo localTimeZone =
            string.IsNullOrWhiteSpace(_options.LocalTimeZone)
                ? TimeZoneInfo.Local
                : TimeZoneInfo.FindSystemTimeZoneById(_options.LocalTimeZone);
        DateTime localTimeNow =
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone);
        logger.LogInformation("Retrieving trade data from {PowerServiceName} for the extract on {LocalTimeNow}, TimeZone: {LocalTimeZone}", nameof(powerService), localTimeNow, localTimeZone);
        
        //trades retrieval.
        var trades = await powerService.GetTradesAsync(localTimeNow);
        
        //Aggregation of trade volume
        var listOfAggregatedTrades = tradeAggregator.Aggregate(trades);
        logger.LogInformation("Trade aggregation completed for the extract on {LocalTimeNow}, TimeZone: {LocalTimeZone}", localTimeNow, localTimeZone);
        
        //Generating report content
        var reportContent = csvBuilder.BuildCsvContent(listOfAggregatedTrades);
        logger.LogInformation("Built csv content for the extract on {LocalTimeNow}, TimeZone: {LocalTimeZone}", localTimeNow, localTimeZone);
        
        var filename = $"PowerPosition_{localTimeNow:yyyyMMdd_HHmm}.csv";
        var report = new Report()
        {
            FileName = filename,
            StoragePath = _options.StoragePath,
            Content = reportContent
        };
        
        //Write to the storage
        await storageStrategy.WriteReportAsync(report, cancellationToken);
        logger.LogInformation("Extract is completed successfully for {LocalTimeNow}, TimeZone: {LocalTimeZone}", localTimeNow, localTimeZone);
    }
    

}