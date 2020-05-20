using System;
using Velo.Server;
using Velo.Server.Handlers;
using Velo.Utils;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class ServerInstaller
    {
        public static DependencyCollection AddServer(this DependencyCollection dependencies)
        {
            if (dependencies.Contains<HttpServer>()) return dependencies;

            dependencies
                .AddSingleton<HttpServer>()
                .AddSingleton<HttpRouter>();

            return dependencies;
        }

        public static DependencyCollection AddFileServer(this DependencyCollection dependencies, string? path = null)
        {
            if (!dependencies.Contains<HttpServer>()) AddServer(dependencies);

            dependencies.AddInstance<IHttpRequestHandler>(new FileRequestHandler(path));
            return dependencies;
        }

        public static DependencyCollection AddHttpHandler<THandler>(this DependencyCollection dependencies,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where THandler : class, IHttpRequestHandler
        {
            dependencies.AddDependency(Typeof<IHttpRequestHandler>.Raw, typeof(THandler), lifetime);
            return dependencies;
        }

        public static DependencyCollection AddHttpHandler(this DependencyCollection dependencies,
            Func<IServiceProvider, IHttpRequestHandler> builder,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var contracts = new[] {typeof(IHttpRequestHandler)};
            dependencies.AddDependency(contracts, builder, lifetime);
            return dependencies;
        }
    }
}