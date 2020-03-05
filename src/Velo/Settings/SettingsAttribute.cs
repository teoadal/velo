using System;

namespace Velo.Settings
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SettingsAttribute : Attribute
    {
        public static bool IsDefined(Type type) => Attribute.IsDefined(type, typeof(SettingsAttribute));
        
        public string Path { get; }

        public SettingsAttribute(string path)
        {
            Path = path;
        }
    }
}