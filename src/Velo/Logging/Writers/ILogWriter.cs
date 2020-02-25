namespace Velo.Logging.Writers
{
    public interface ILogWriter
    {
        LogLevel Level { get; }

        void Write(LogLevel level, string message);
    }
}