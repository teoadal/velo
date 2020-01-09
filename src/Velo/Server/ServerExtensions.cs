using System.Reflection;
using Velo.DependencyInjection;
using Velo.Server.Handlers;

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

        public static DependencyCollection AddFileServer(this DependencyCollection collection, string path = "")
        {
            if (!collection.Contains<HttpServer>()) AddServer(collection);
            
            collection.AddSingleton<IHttpRequestHandler>(s => new FileRequestHandler(path));
            return collection;
        }

        public static DependencyCollection AddHttpController<TController>(this DependencyCollection collection)
            where TController: class
        {
            var properties = typeof(TController).GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                
            }
            
            
            return collection;
        }
        
        public static DependencyCollection AddHttpHandler<THandler>(this DependencyCollection collection)
            where THandler : class, IHttpRequestHandler
        {
            collection.AddSingleton<IHttpRequestHandler, THandler>();
            return collection;
        }
    }
}