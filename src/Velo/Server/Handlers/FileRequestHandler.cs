using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.Server.Handlers
{
    internal sealed class FileRequestHandler : IHttpRequestHandler
    {
        private readonly string _root;

        public FileRequestHandler(string path)
        {
            _root = string.IsNullOrWhiteSpace(path)
                ? Environment.CurrentDirectory
                : Path.Combine(Environment.CurrentDirectory, path);
        }

        public bool Applicable(HttpVerb verb, Uri address)
        {
            return verb == HttpVerb.Get && Path.HasExtension(address.LocalPath);
        }

        public Task Handle(HttpListenerContext context, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(_root, context.Request.Url.LocalPath.Substring(1));

            if (File.Exists(filePath))
            {
                using var fileStream = File.OpenRead(filePath);
                fileStream.CopyTo(context.Response.OutputStream, 1024);
            }
            else
            {
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            }

            return TaskUtils.CompletedTask;
        }
    }
}