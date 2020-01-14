namespace Velo.Settings
{
    public interface IConfiguration
    {
        string GetString(string path);
    }
}