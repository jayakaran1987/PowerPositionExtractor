using Microsoft.Extensions.Hosting;
using Serilog;

namespace PowerPositionExtractor.Worker.Startup;

public class LoggerStartup
{
    public static void Configure(IHostBuilder builder)
    {
        // Configure Serilog for logging 
        builder.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));
    }
}