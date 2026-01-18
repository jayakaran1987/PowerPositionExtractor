using PowerPositionExtractor.Logic.Interfaces;
using PowerPositionExtractor.Logic.Model;

namespace PowerPositionExtractor.Logic.Storage;

public class GcsStorageStrategy:IStorageStrategy
{
    public Task WriteReportAsync(Report report, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}