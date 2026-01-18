using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerPositionExtractor.Logic.Model.Options;

namespace PowerPositionExtractor.Worker.Startup;

public static class SettingsStartup
{
    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            //helm values appsettings are configured to store appsettings.k8s.json
            config.AddJsonFile("appsettings.k8s.json", optional: true, reloadOnChange: true);
        });
        // Options pattern to bind the configuration and added to DI
        builder.ConfigureServices((hostingContext, services) =>
        {
            services.Configure<ExtractorOptions>(hostingContext.Configuration.GetSection(ExtractorOptions.ConfigKey));
        });
    }
}