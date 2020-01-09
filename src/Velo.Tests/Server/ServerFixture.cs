using System;
using Velo.DependencyInjection;

namespace Velo.Server
{
    public class ServerFixture : IDisposable
    {
        public readonly int Port = 8512;
        
        public readonly HttpServer Server;
        
        public ServerFixture()
        {
            var provider = new DependencyCollection()
                .AddFileServer()
                .BuildProvider();

            Server = provider.GetService<HttpServer>();
            Server.Start(Port);
        }
        
        public void Dispose()
        {
            Server.Stop();
        }
    }
}