using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.Server
{
    public interface IHttpRequestHandler
    {
        public bool Applicable(HttpVerb verb, Uri address);

        public Task Handle(HttpListenerContext context, CancellationToken cancellationToken);
    }
}