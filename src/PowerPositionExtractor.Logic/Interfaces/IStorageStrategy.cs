using PowerPositionExtractor.Logic.Model;

namespace PowerPositionExtractor.Logic.Interfaces;

public interface IStorageStrategy
{
    Task WriteReportAsync(Report report, CancellationToken cancellationToken = default);
}