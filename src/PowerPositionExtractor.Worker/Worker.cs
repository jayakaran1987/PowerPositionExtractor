using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PowerPositionExtractor.Logic.Model;
using PowerPositionExtractor.Logic.Model.Options;
using PowerPositionExtractor.Worker.Jobs;
using Quartz;
namespace PowerPositionExtractor.Worker;

public class Worker(
    ILogger<Worker> logger,
    IServiceProvider serviceProvider,
    IOptions<ExtractorOptions> extractorOptions) : BackgroundService
{
    //Quartz used for some good features Missfire handling, wait on complete, if needed can consider using job persistant as well
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Starting worker {nameof(Worker)}");
        if (extractorOptions.Value == null)
            throw new ArgumentNullException(nameof(extractorOptions));
        if (extractorOptions.Value.ScheduledTimeIntervalInMinutes <= 0)
            throw new ArgumentException("ScheduledTimeIntervalInMinutes must be greater than 0 minutes");
        if (extractorOptions.Value.ScheduledTimeIntervalInMinutes > 60)
            throw new ArgumentException("Report should be generated every hour. ScheduledTimeIntervalInMinutes must be less than 60 minutes");
        using var scope = serviceProvider.CreateScope();
        try
        {
            var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler(stoppingToken);
            var extractionJob = JobBuilder.Create<ExtractionJob>().Build();
            // The job is executed immediately after the scheduler discovers misfire situation
            var trigger = TriggerBuilder
                .Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(extractorOptions.Value.ScheduledTimeIntervalInMinutes) 
                    .RepeatForever()
                    .WithMisfireHandlingInstructionFireNow())
                .Build();
            await scheduler.ScheduleJob(extractionJob, trigger, stoppingToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{nameof(Worker)} failed due to Exception: {e.Message}");
        }
    }
}