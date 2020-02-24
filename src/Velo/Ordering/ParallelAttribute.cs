using System;

namespace Velo.Ordering
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ParallelAttribute : Attribute
    {
    }
}