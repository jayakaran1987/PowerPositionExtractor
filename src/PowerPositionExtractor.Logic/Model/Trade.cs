namespace PowerPositionExtractor.Logic.Model;

public record Trade(
    int Period,
    string LocalTime,
    double AggregatedVolume);