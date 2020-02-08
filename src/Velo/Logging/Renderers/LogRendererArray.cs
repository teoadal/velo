using System;
using System.IO;
using System.Text;
using Velo.Collections;
using Velo.Serialization;
using Velo.Serialization.Converters;

namespace Velo.Logging.Renderers
{
    internal sealed class LogRendererArray: LogRenderer
    {
        private readonly IJsonConverter[] _converters;
        
        public LogRendererArray(string template, StringBuilder stringBuilder, IConvertersCollection converters, LocalList<Type> argumentTypes) 
            : base(template, stringBuilder)
        {
            var argumentConverters = new IJsonConverter[argumentTypes.Length];
            for (var i = 0; i < argumentTypes.Length; i++)
            {
                argumentConverters[i] = converters.Get(argumentTypes[i]);
            }

            _converters = argumentConverters;
        }

        public void Render(TextWriter writer, object[] arguments)
        {
            var counter = 0;
            foreach (var part in Parts)
            {
                if (part.IsArgument)
                {
                    var argumentConverter = _converters[counter];
                    argumentConverter.Serialize(arguments[counter], writer);
                    counter++;
                }
                else writer.Write(part.Text);
            }
        }
    }
}