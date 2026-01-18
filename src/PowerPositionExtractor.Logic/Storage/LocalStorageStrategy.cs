using System.Text;
using Microsoft.Extensions.Logging;
using PowerPositionExtractor.Logic.Interfaces;
using PowerPositionExtractor.Logic.Model;

namespace PowerPositionExtractor.Logic.Storage;

public class LocalStorageStrategy(ILogger<LocalStorageStrategy> logger):IStorageStrategy 
{
    public async Task WriteReportAsync(Report report, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Executing {nameof(LocalStorageStrategy)} and {nameof(WriteReportAsync)}");
        
        var filepath = Path.Combine(report.StoragePath, report.FileName);
        var directory = Path.GetDirectoryName(filepath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            logger.LogInformation("Directory doesn't exists. Creating directory: {Directory}", directory);
            Directory.CreateDirectory(directory);
        }
        try
        {
            await File.WriteAllTextAsync(filepath, report.Content, Encoding.UTF8, cancellationToken);
            logger.LogInformation("Successfully wrote CSV file: {FilePath})", filepath);
        }
        catch (Exception e)
        {
            logger.LogError( e, "Error occurred while writing CSV file. FilePath: {FilePath}", filepath);
            throw;
        }
    }
}