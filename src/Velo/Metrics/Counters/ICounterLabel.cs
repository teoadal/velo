namespace Velo.Metrics.Counters
{
    public interface ICounterLabel
    {
        string Name { get; }
        
        double Value { get; }

        double Increment(double value = 1d);

        double IncrementTo(double targetValue);

        double Flush();
    }
}