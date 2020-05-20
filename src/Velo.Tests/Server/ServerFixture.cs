using System;
using Velo.DependencyInjection;
using Velo.Server;

namespace Velo.Tests.Server
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

            Server = provider.GetRequired<HttpServer>();
            Server.Start(Port);
        }
        
        public void Dispose()
        {
            Server.Stop();
        }
    }
}