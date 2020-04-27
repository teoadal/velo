using System;

namespace Velo.Threading
{
    /// <summary>
    /// Allow use class for parallel handling (if possible).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public sealed class ParallelAttribute : Attribute
    {
        public static bool IsDefined(Type type) => Attribute.IsDefined(type, typeof(ParallelAttribute));
    }
}