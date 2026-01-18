using PowerPositionExtractor.Logic.Model.Enum;

namespace PowerPositionExtractor.Logic.Model.Options;

public class ExtractorOptions
{
    public const string ConfigKey = "ExtractorOptions";
    public int ScheduledTimeIntervalInMinutes { get; set; }
    public string? StoragePath { get; set; }
    public string? LocalTimeZone{ get; set; }
    public StorageStrategyEnum StorageStrategy { get; set; }
}