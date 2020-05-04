using System;
using System.Reflection;

namespace Velo.Serialization.Attributes
{
    /// <summary>
    /// Ignore property on serialization
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        public static bool IsDefined(PropertyInfo property)
        {
            return Attribute.IsDefined(property, typeof(IgnoreAttribute));
        }
    }
}