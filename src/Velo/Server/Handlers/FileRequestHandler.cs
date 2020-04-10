using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Velo.Threading;

namespace Velo.Server.Handlers
{
    internal sealed class FileRequestHandler : IHttpRequestHandler
    {
        private readonly string _root;

        public FileRequestHandler(string path)
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _root = string.IsNullOrWhiteSpace(path)
                ? currentDirectory
                : Path.Combine(currentDirectory, path);
        }

        public bool Applicable(HttpVerb verb, Uri address)
        {
            return verb == HttpVerb.Get && Path.HasExtension(address.LocalPath);
        }

        public Task Handle(HttpListenerContext context, CancellationToken cancellationToken)
        {
            var localPath = context.Request.Url.LocalPath;
            var filePath = Path.Combine(_root, localPath[0] == '/' ? localPath.Substring(1) : localPath);

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