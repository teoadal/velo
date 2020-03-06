using System.IO;
using System.Text;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.Settings.Sources
{
    internal sealed class JsonFileSource : ISettingsSource
    {
        private readonly string _path;
        private readonly bool _required;

        public JsonFileSource(string path, bool required)
        {
            _path = path;
            _required = required;
        }

        public bool TryGet(out JsonObject data)
        {
            if (!File.Exists(_path))
            {
                if (_required) throw Error.FileNotFound(_path);
                data = null;
                return false;
            }

            data = (JsonObject) JsonData.Parse(File.OpenRead(_path), Encoding.UTF8);
            return true;
        }
    }
}