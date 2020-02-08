namespace Velo.Logging
{
    public interface ILogWriter
    {
        LogLevel Level { get; }

        void Write(LogLevel level, string message);
    }
}