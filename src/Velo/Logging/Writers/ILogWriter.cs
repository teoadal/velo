using Velo.Serialization.Models;

namespace Velo.Logging.Writers
{
    public interface ILogWriter
    {
        LogLevel Level { get; }

        void Write(LogContext context, JsonObject message);
    }
}