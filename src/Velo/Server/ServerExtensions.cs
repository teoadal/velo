using System;
using Velo.DependencyInjection;
using Velo.Server.Handlers;
using Velo.Utils;

namespace Velo.Server
{
    public static class ServerExtensions
    {
        public static DependencyCollection AddServer(this DependencyCollection collection)
        {
            if (collection.Contains<HttpServer>()) return collection;

            collection
                .AddSingleton<HttpServer>()
                .AddSingleton<HttpRouter>();

            return collection;
        }

        public static DependencyCollection AddFileServer(this DependencyCollection collection, string path = null)
        {
            if (!collection.Contains<HttpServer>()) AddServer(collection);

            collection.AddInstance<IHttpRequestHandler>(new FileRequestHandler(path));
            return collection;
        }

        public static DependencyCollection AddHttpHandler<THandler>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where THandler : class, IHttpRequestHandler
        {
            collection.AddDependency(Typeof<IHttpRequestHandler>.Raw, typeof(THandler), lifetime);
            return collection;
        }

        public static DependencyCollection AddHttpHandler(this DependencyCollection collection,
            Func<IDependencyScope, IHttpRequestHandler> builder,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            collection.AddDependency(builder, lifetime);
            return collection;
        }
    }
}