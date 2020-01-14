using System.Collections.Generic;
using System.IO;

namespace Velo.Settings
{
    public class Configuration : IConfiguration
    {
        private Dictionary<string, string> _values;

        public Configuration(Stream stream)
        {
            stream.Dispose();
        }

        public Configuration(string filePath = "appsettings.json")
            : this(File.Exists(filePath) ? File.OpenRead(filePath) : Stream.Null)
        {
        }

        public string GetString(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}