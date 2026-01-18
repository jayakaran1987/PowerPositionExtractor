using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using PowerPositionExtractor.Logic.Interfaces;
using Quartz;

namespace PowerPositionExtractor.Worker.Jobs;

[DisallowConcurrentExecution]
public class ExtractionJob(ILogger<ExtractionJob> logger, IOrchestrator orchestrator): IJob
{
    //Retry added.
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation($"The {nameof(ExtractionJob)} is starting");
        var policy = CreateRetryPolicyAsync();
        await policy.ExecuteAsync(async() => await orchestrator.RunOrchestratorAsync(context.CancellationToken));
        logger.LogInformation($"Executing {nameof(ExtractionJob)} is finished. NextFireTimeUtc: {context.NextFireTimeUtc}");
    }
    
    private AsyncRetryPolicy CreateRetryPolicyAsync(int maxRetry = 3)
    {
        return Policy.Handle<Exception>(ex => ex is not ArgumentNullException || ex is not ArgumentException)
            .WaitAndRetryAsync(
                maxRetry,
                retryAttempt => TimeSpan.FromSeconds(2*retryAttempt),
                onRetry: (exception, timeSpan, retryCount) =>
                {
                    logger.LogWarning(exception, "Retry {retryCount} and waiting {timeSpan} seconds", retryCount,timeSpan );
                });
    }
}