using System;
using System.Collections.Generic;

namespace Velo.Server
{
    internal sealed class HttpRouter
    {
        private readonly IHttpRequestHandler[] _requestHandlers;
        private readonly Dictionary<Definition, IHttpRequestHandler> _concreteHandlers;
        private readonly Type _verbType;

        public HttpRouter(IHttpRequestHandler[] requestHandlers)
        {
            _requestHandlers = requestHandlers;
            _concreteHandlers = new Dictionary<Definition, IHttpRequestHandler>(new DefinitionComparer());
            _verbType = typeof(HttpVerb);
        }

        public bool TryGetHandler(string method, Uri url, out IHttpRequestHandler handler)
        {
            var definition = new Definition(method, url);
            if (_concreteHandlers.TryGetValue(definition, out handler)) return true;

            var httpMethod = (HttpVerb) Enum.Parse(_verbType, method, true);
            foreach (var requestHandler in _requestHandlers)
            {
                if (!requestHandler.Applicable(httpMethod, url)) continue;

                _concreteHandlers.Add(definition, requestHandler);
                handler = requestHandler;
                break;
            }

            return handler != null;
        }

        private readonly struct Definition
        {
            public readonly string Method;
            public readonly Uri Url;

            public Definition(string method, Uri url)
            {
                Method = method;
                Url = url;
            }
        }

        private sealed class DefinitionComparer : IEqualityComparer<Definition>
        {
            public bool Equals(Definition first, Definition second)
            {
                return first.Method.Equals(second.Method) && first.Url.Equals(second.Url);
            }

            public int GetHashCode(Definition def)
            {
                unchecked
                {
                    return (def.Method.GetHashCode() * 397) ^ def.Url.GetHashCode();
                }
            }
        }
    }
}