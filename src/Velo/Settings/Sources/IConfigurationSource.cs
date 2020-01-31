using Velo.Serialization.Models;

namespace Velo.Settings.Sources
{
    internal interface IConfigurationSource
    {
        JsonObject FetchData();
    }
}