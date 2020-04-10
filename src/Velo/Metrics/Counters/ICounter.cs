namespace Velo.Metrics.Counters
{
    public interface ICounter
    {
        string Description { get; }

        ICounterLabel[] Labels { get; }

        string Name { get; }

        double Increment(string label, double value = 1d);

        double IncrementTo(string label, double targetValue);

        double this[string label] { get; }
    }

    // ReSharper disable once UnusedTypeParameter
    public interface ICounter<TSender> : ICounter
    {
    }
}