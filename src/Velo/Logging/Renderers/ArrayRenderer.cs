using System;
using Velo.Collections;
using Velo.Serialization;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;

namespace Velo.Logging.Renderers
{
    internal sealed class ArrayRenderer : Renderer
    {
        private readonly IJsonConverter[] _converters;

        public ArrayRenderer(string[] arguments, LocalList<Type> argumentTypes, IConvertersCollection converters)
            : base(arguments)
        {
            var argumentConverters = new IJsonConverter[argumentTypes.Length];
            for (var i = 0; i < argumentTypes.Length; i++)
            {
                argumentConverters[i] = converters.Get(argumentTypes[i]);
            }

            _converters = argumentConverters;
        }

        public void Render(JsonObject message, object[] arguments)
        {
            for (var i = 0; i < Arguments.Length; i++)
            {
                var value = _converters[i].Write(arguments[i]);
                message.Add(Arguments[i], value);
            }
        }
    }
}