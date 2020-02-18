using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.Server
{
    public interface IHttpRequestHandler
    {
        bool Applicable(HttpVerb verb, Uri address);

        Task Handle(HttpListenerContext context, CancellationToken cancellationToken);
    }
}