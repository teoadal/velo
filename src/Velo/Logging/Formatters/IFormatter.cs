using System.IO;
using Velo.Serialization.Models;

namespace Velo.Logging.Formatters
{
    internal interface IFormatter
    {
        void Write(JsonObject message, TextWriter writer);
    }
}