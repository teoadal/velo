using Velo.Serialization.Models;

namespace Velo.Settings.Sources
{
    internal interface IConfigurationSource
    {
        bool TryGet(out JsonObject data);
    }
}