using System;
using Velo.Dependencies;

namespace Velo.Utils
{
    internal static class Error
    {
        public static InvalidOperationException CircularDependency(IDependency dependency)
        {
            return new InvalidOperationException($"Detected circular dependency {dependency}");
        }
        
    }
}