using Microsoft.Extensions.Hosting;
using Quartz;

namespace PowerPositionExtractor.Worker.Startup;

public static class SchedulerStartup
{
    public static void Configure(IHostBuilder builder)
    {
        // Configure Quartz
        builder.ConfigureServices(services =>
        {
            services.AddQuartz();
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        });
    }
}