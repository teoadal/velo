using System;
using System.Collections.Generic;
using System.Text;

using Velo.Serialization.Converters;

namespace Velo.Serialization
{
    public sealed class JSerializer
    {
        private static StringBuilder _buffer;
        private readonly Dictionary<Type, IJsonConverter> _converters;

        public JSerializer()
        {
            _converters = new Dictionary<Type, IJsonConverter>
            {
                {typeof(bool), new BoolConverter()},
                {typeof(double), new DoubleConverter()},
                {typeof(float), new FloatConverter()},
                {typeof(int), new IntConverter()},
                {typeof(string), new StringConverter()},
            };
        }

        public T Deserialize<T>(string data)
        {
            if (_buffer == null) _buffer = new StringBuilder(200);

            var tokenizer = new JTokenizer(data, _buffer);
            foreach (var token in tokenizer)
            {
            }

            return default;
        }

        public string Serialize(object data)
        {
            throw new NotImplementedException();
        }

        private IJsonConverter BuildConverter(Type type)
        {
            throw new NotImplementedException();
        }
    }
}