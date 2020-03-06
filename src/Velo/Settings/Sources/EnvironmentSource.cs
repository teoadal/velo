using System;
using System.Collections;
using Velo.Serialization.Models;

namespace Velo.Settings.Sources
{
    internal sealed class EnvironmentSource : ISettingsSource
    {
        public bool TryGet(out JsonObject data)
        {
            var variables = Environment.GetEnvironmentVariables();

            data = new JsonObject();
            foreach (DictionaryEntry entry in variables)
            {
                if (entry.Value is string stringValue)
                {
                    data.Add(entry.Key.ToString(), JsonValue.String(stringValue));
                }
            }

            return true;
        }
    }
}