using System;
using Velo.Collections.Local;
using Velo.Logging.Formatters;
using Velo.Serialization;
using Velo.Serialization.Models;

namespace Velo.Logging.Renderers
{
    internal sealed class ArrayRenderer : Renderer
    {
        private readonly string[] _arguments;
        private readonly IJsonConverter[] _converters;

        public ArrayRenderer(string[] arguments, ILogFormatter formatter, LocalList<Type> argumentTypes, IConvertersCollection converters)
            : base(formatter)
        {
            var argumentConverters = new IJsonConverter[argumentTypes.Length];
            for (var i = 0; i < argumentConverters.Length; i++)
            {
                argumentConverters[i] = converters.Get(argumentTypes[i]);
            }

            _arguments = arguments;
            _converters = argumentConverters;
        }

        public void Render(JsonObject message, object[] arguments)
        {
            try
            {
                for (var i = _arguments.Length - 1; i >= 0; i--)
                {
                    var value = _converters[i].WriteObject(arguments[i]);
                    message.Add(_arguments[i], value);
                }
            }
            catch (InvalidCastException e)
            {
                throw new InvalidTemplateException(e);
            }
        }
    }
}