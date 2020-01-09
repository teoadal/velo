using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.Server.Handlers
{
    internal sealed class FileRequestHandler : IHttpRequestHandler
    {
        private readonly string _fileDirectory;
        
        public FileRequestHandler(string path)
        {
            _fileDirectory = Path.Combine(Environment.CurrentDirectory, path);
        }

        public bool Applicable(HttpVerb verb, Uri address)
        {
            return verb == HttpVerb.Get && Path.HasExtension(address.LocalPath);
        }

        public Task Handle(HttpListenerContext context, CancellationToken cancellationToken)
        {
            var filePath = Path.Join(_fileDirectory, context.Request.Url.LocalPath);
            
            if (File.Exists(filePath))
            {
                using var fileStream = File.OpenRead(filePath);
                fileStream.CopyTo(context.Response.OutputStream, 1024);
            }
            else
            {
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            }

            return Task.CompletedTask;
        }
    }
}