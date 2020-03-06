using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Velo.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static TService[] GetArray<TService>(this IServiceProvider provider)
            where TService: class
        {
            return provider.GetServices<TService>().ToArray();
        }
    }
}