namespace Velo.Settings
{
    internal class ConfigurationSection<T> : ISettings<T>
    {
        private readonly IConfiguration _configuration;
        private readonly string _path;

        public ConfigurationSection(IConfiguration configuration, string path)
        {
            _configuration = configuration;
            _path = path;
        }

        public T Get()
        {
            return _configuration.Get<T>(_path);
        }
    }
}