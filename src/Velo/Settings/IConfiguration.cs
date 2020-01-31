namespace Velo.Settings
{
    public interface IConfiguration
    {
        string Get(string path);

        T Get<T>(string path);

        void Reload();
    }
}