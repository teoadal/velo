using System;

namespace Velo.Settings
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SettingsAttribute : Attribute
    {
        public string Path { get; }

        public SettingsAttribute(string path)
        {
            Path = path;
        }
    }
}