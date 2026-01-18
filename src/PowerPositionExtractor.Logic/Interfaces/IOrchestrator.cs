namespace PowerPositionExtractor.Logic.Interfaces;

public interface IOrchestrator
{
    Task RunOrchestratorAsync(CancellationToken cancellationToken);
}