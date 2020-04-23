using System;
using Velo.Logging.Formatters;
using Velo.Serialization;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;

namespace Velo.Logging.Renderers
{
    internal abstract class Renderer
    {
        public readonly ILogFormatter Formatter;
        
        [ThreadStatic]
        private static JsonObject? _buffer;
        
        protected Renderer(ILogFormatter formatter)
        {
            Formatter = formatter;
        }
        
        public static JsonObject GetBuffer(int capacity)
        {
            if (_buffer == null) return new JsonObject(capacity);
            
            var result = _buffer;
            _buffer = null;
            
            return result;
        }

        public static void ReturnBuffer(JsonObject message)
        {
            message.Clear();
            _buffer = message;
        }
    }

    internal sealed class Renderer<T1> : Renderer
    {
        private readonly string _argument;
        private readonly IJsonConverter<T1> _converter;

        public Renderer(string[] arguments, ILogFormatter formatter, IConvertersCollection converters) 
            : base(formatter)
        {
            _argument = arguments[0];
            _converter = converters.Get<T1>();
        }

        public void Render(JsonObject message, T1 arg)
        {
            message[_argument] = _converter.Write(arg);
        }
    }

    internal sealed class Renderer<T1, T2> : Renderer
    {
        private readonly string _argument1;
        private readonly IJsonConverter<T1> _converter1;
        private readonly string _argument2;
        private readonly IJsonConverter<T2> _converter2;

        public Renderer(string[] arguments, ILogFormatter formatter, IConvertersCollection converters)
            : base(formatter)
        {
            _argument1 = arguments[0];
            _converter1 = converters.Get<T1>();
            _argument2 = arguments[1];
            _converter2 = converters.Get<T2>();
        }

        public void Render(JsonObject message, T1 arg1, T2 arg2)
        {
            message.Add(_argument1, _converter1.Write(arg1));
            message.Add(_argument2, _converter2.Write(arg2));
        }
    }

    internal sealed class Renderer<T1, T2, T3> : Renderer
    {
        private readonly string _argument1;
        private readonly IJsonConverter<T1> _converter1;
        private readonly string _argument2;
        private readonly IJsonConverter<T2> _converter2;
        private readonly string _argument3;
        private readonly IJsonConverter<T3> _converter3;
        
        public Renderer(string[] arguments, ILogFormatter formatter, IConvertersCollection converters)
            : base(formatter)
        {
            _argument1 = arguments[0];
            _converter1 = converters.Get<T1>();
            _argument2 = arguments[1];
            _converter2 = converters.Get<T2>();
            _argument3 = arguments[2];
            _converter3 = converters.Get<T3>();
        }

        public void Render(JsonObject message, T1 arg1, T2 arg2, T3 arg3)
        {
            message.Add(_argument1, _converter1.Write(arg1));
            message.Add(_argument2, _converter2.Write(arg2));
            message.Add(_argument3, _converter3.Write(arg3));
        }
    }
}