using Microsoft.Extensions.Options;
using PowerPositionExtractor.Logic.Model;

namespace PowerPositionExtractor.Logic.Interfaces;

public interface ICsvBuilder
{
    string BuildCsvContent(List<Trade> data);
}