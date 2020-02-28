using System.IO;
using Velo.Serialization.Models;

namespace Velo.Logging.Formatters
{
    public interface ILogFormatter
    {
        void Write(JsonObject message, TextWriter writer);
    }
}