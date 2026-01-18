
using System.Text;
using Microsoft.Extensions.Logging;
using PowerPositionExtractor.Logic.Interfaces;
using PowerPositionExtractor.Logic.Model;

namespace PowerPositionExtractor.Logic.Features;

public class CsvBuilder(ILogger<CsvBuilder> logger): ICsvBuilder
{
    public string BuildCsvContent(List<Trade> data)
    {
        var csv = new StringBuilder();
        // CSV output format must be two columns and the first row must be a header row.
        csv.AppendLine("Local Time,Volume");
        foreach (var entry in data.OrderBy(o => o.Period))
        {
            csv.AppendLine($"{entry.LocalTime},{entry.AggregatedVolume}");
        }
        return csv.ToString();
    }
}