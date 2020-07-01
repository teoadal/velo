using Velo.Settings.Provider;

namespace Velo.Settings
{
    public static class SettingsExtensions
    {
        public static string GetConnectionString(this ISettingsProvider settingsProvider, string connectionStringName)
        {
            return settingsProvider.Get($"ConnectionStrings.{connectionStringName}");
        }
    }
}