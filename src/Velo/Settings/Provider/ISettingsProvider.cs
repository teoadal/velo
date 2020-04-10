namespace Velo.Settings.Provider
{
    public interface ISettingsProvider
    {
        bool Contains(string path);

        string Get(string path);

        T Get<T>(string path);

        void Reload();

        bool TryGet<T>(string path, out T value);
    }
}