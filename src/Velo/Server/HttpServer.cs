using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Velo.DependencyInjection;
using Velo.Utils;

namespace Velo.Server
{
    public sealed class HttpServer : IDisposable
    {
        private readonly HttpRouter _router;
        private readonly DependencyProvider _dependencyProvider;

        private bool _disposed;
        private HttpListener? _httpListener;
        private Task? _listenerTask;
        private bool _isStarted;

        internal HttpServer(HttpRouter router, DependencyProvider dependencyProvider)
        {
            _router = router;
            _dependencyProvider = dependencyProvider;
        }

        public void Start(int port = 8125)
        {
            if (_disposed) throw Error.Disposed(nameof(HttpServer));
            if (_isStarted) throw Error.InvalidOperation("Server already started");

            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://*:{port}/");

            _listenerTask = Task.Run(Listen);

            _isStarted = true;
        }

        public void Stop()
        {
            if (!_isStarted) throw Error.InvalidOperation("Server not started");

            _httpListener?.Stop();
            _listenerTask?.Wait();
            _httpListener = null;

            _isStarted = false;
        }

        private async Task Listen()
        {
            if (_httpListener == null) throw Error.InvalidOperation("HttpListener isn't created");

            _httpListener.Start();

            while (_httpListener != null && _httpListener.IsListening)
            {
                HttpListenerContext context;
                try
                {
                    context = await _httpListener.GetContextAsync();
                }
                catch (ObjectDisposedException)
                {
                    return;
                }

                var httpRequest = context.Request;
                var httpResponse = context.Response;

                if (_router.TryGetHandler(httpRequest.HttpMethod, httpRequest.Url, out var handler))
                {
                    httpResponse.StatusCode = (int) HttpStatusCode.OK;
                    await HandleRequest(context, handler);
                }
                else
                {
                    httpResponse.StatusCode = (int) HttpStatusCode.NotFound;
                }

                httpResponse.Close();
            }
        }

        private async Task HandleRequest(HttpListenerContext context, IHttpRequestHandler handler)
        {
            using var cancellationSource = new CancellationTokenSource();
            using var dependencyScope = _dependencyProvider.CreateScope();

            try
            {
                await handler.Handle(context, cancellationSource.Token);
            }
            catch (Exception)
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            if (_isStarted) Stop();

            _disposed = true;
        }
    }
}