using Velo.Utils;

namespace Velo.Settings.Provider
{
    internal sealed class NullProvider : ISettings
    {
        public bool Contains(string path)
        {
            return false;
        }

        public string Get(string path)
        {
            throw Error.NotFound($"Configuration path '{path}' not found");
        }

        public T Get<T>(string path)
        {
            throw Error.NotFound($"Configuration path '{path}' not found");
        }

        public void Reload()
        {
        }

        public bool TryGet<T>(string path, out T value)
        {
            value = default;
            return false;
        }
    }
}