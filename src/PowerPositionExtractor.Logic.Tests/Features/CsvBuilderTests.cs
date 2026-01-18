using System.Text;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PowerPositionExtractor.Logic.Features;
using PowerPositionExtractor.Logic.Model;
using Shouldly;

namespace PowerPositionExtractor.Logic.Tests.Features;

public class CsvBuilderTests
{
    private readonly CsvBuilder _feature;
    public CsvBuilderTests()
    {
        var logger = Substitute.For<ILogger<CsvBuilder>>();
        _feature = new CsvBuilder(logger);
    }
    
    [Fact]
    public void Given_ListOfTrade_When_BuildCsvContent_Then_ReturnExpected()
    {
        //Given
        var data = new List<Trade>
                {
                    new Trade ( 2,  "00:00",  140 ),
                    new Trade (3, "01:00",  130),
                    new Trade (1, "23:00", 150 ),
                };
        var expectedResult = new StringBuilder()
            .AppendLine("Local Time,Volume")
            .AppendLine("23:00,150")
            .AppendLine("00:00,140")
            .AppendLine("01:00,130").ToString();
        //When 
        var result = _feature.BuildCsvContent(data);
        //Then
        result.ShouldBe(expectedResult);
    }
    
    [Fact]
    public void Given_EmptyListOfTrade_When_BuildCsvContent_Then_ReturnHeader()
    {
        //Given
        var data = new List<Trade>();
        var expectedResult = new StringBuilder().AppendLine("Local Time,Volume").ToString();
        //When 
        var result = _feature.BuildCsvContent(data);
        //Then
        result.ShouldBe(expectedResult);
    }
}