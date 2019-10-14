using System.Collections.Generic;
using Velo.DependencyInjection.Resolvers;

namespace Velo.DependencyInjection
{
    internal readonly struct DependencyDescription
    {
        public readonly DependencyResolver Main;

        public readonly List<DependencyResolver> OtherResolvers;

        public readonly int ResolversCount;

        public DependencyDescription(DependencyResolver main)
        {
            ResolversCount = 1;

            Main = main;
            OtherResolvers = null;
        }

        private DependencyDescription(DependencyResolver main, List<DependencyResolver> otherResolvers)
        {
            ResolversCount = 1 + otherResolvers.Count;

            Main = main;
            OtherResolvers = otherResolvers;
        }

        public DependencyDescription Add(DependencyResolver resolver)
        {
            var descriptions = OtherResolvers ?? new List<DependencyResolver>();
            descriptions.Add(resolver);
            return new DependencyDescription(Main, descriptions);
        }
    }
}