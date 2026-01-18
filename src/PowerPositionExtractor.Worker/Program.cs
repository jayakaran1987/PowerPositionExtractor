
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Hosting;
using PowerPositionExtractor.Logic.Core;
using PowerPositionExtractor.Logic.Features;
using PowerPositionExtractor.Logic.Interfaces;
using PowerPositionExtractor.Logic.Model.Enum;
using PowerPositionExtractor.Logic.Model.Options;
using PowerPositionExtractor.Logic.Storage;
using PowerPositionExtractor.Worker;
using PowerPositionExtractor.Worker.Startup;
using Services;

var builder = Host.CreateDefaultBuilder(args);
SettingsStartup.Configure(builder);
SchedulerStartup.Configure(builder);
LoggerStartup.Configure(builder);

// Configure Dependency Injection of service
builder.ConfigureServices((context, services) =>
{
    var extractorOptions = context.Configuration.GetSection(ExtractorOptions.ConfigKey).Get<ExtractorOptions>();
    if (extractorOptions != null)
    {
        switch (extractorOptions.StorageStrategy)
        {
            case StorageStrategyEnum.Gcs:
                services.AddScoped<IStorageStrategy, GcsStorageStrategy>();
                break;
            case StorageStrategyEnum.Local:
            default:
                services.AddScoped<IStorageStrategy, LocalStorageStrategy>();
                break;
        }
    }
    services.AddHostedService<Worker>();
    services.AddScoped<IPowerService, PowerService>();
    services.AddScoped<ICsvBuilder, CsvBuilder>();
    services.AddScoped<IOrchestrator, Orchestrator>();
    services.AddScoped<ITradeAggregator, TradeAggregator>();
});

var host = builder.Build();
await host.RunAsync();