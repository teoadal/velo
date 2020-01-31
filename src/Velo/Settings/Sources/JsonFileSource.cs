using System.IO;
using System.Text;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.Settings.Sources
{
    internal sealed class JsonFileSource : IConfigurationSource
    {
        private readonly string _path;
        private readonly bool _required;

        public JsonFileSource(string path, bool required)
        {
            _path = path;
            _required = required;
        }

        public JsonObject FetchData()
        {
            if (!File.Exists(_path))
            {
                if (_required) throw Error.FileNotFound(_path);
                return null;
            }

            var data = JsonData.Parse(File.OpenRead(_path), Encoding.UTF8);
            return (JsonObject) data;
        }
    }
}