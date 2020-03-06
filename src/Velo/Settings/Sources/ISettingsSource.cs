using Velo.Serialization.Models;

namespace Velo.Settings.Sources
{
    internal interface ISettingsSource
    {
        bool TryGet(out JsonObject data);
    }
}