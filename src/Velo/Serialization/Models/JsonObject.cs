using System.Collections.Generic;
using System.IO;
using Velo.Extensions;
using Velo.Utils;

namespace Velo.Serialization.Models
{
    public sealed class JsonObject : JsonData
    {
        private readonly Dictionary<string, JsonData> _properties;

        public JsonObject() : base(JsonDataType.Object)
        {
            _properties = new Dictionary<string, JsonData>(StringUtils.IgnoreCaseComparer);
        }

        public JsonObject(int capacity) : base(JsonDataType.Object)
        {
            _properties = new Dictionary<string, JsonData>(capacity);
        }

        public void Add(string property, JsonData value)
        {
            _properties.Add(string.Intern(property), value);
        }

        public void Clear()
        {
            _properties.Clear();
        }

        public bool Contains(string property)
        {
            return _properties.ContainsKey(property);
        }

        public Dictionary<string, JsonData>.Enumerator GetEnumerator() => _properties.GetEnumerator();

        public bool Remove(string property)
        {
            return _properties.Remove(property);
        }

        public override void Serialize(TextWriter writer)
        {
            writer.Write('{');

            var first = true;
            foreach (var (propertyName, propertyValue) in _properties)
            {
                if (first) first = false;
                else writer.Write(',');

                writer.Write('\"');
                writer.Write(propertyName);
                writer.Write("\":");

                propertyValue.Serialize(writer);
            }

            writer.Write('}');
        }

        public bool TryGet(string property, out JsonData value)
        {
            return _properties.TryGetValue(property, out value);
        }

        public JsonData this[string property]
        {
            get => _properties[property];
            set => _properties[property] = value;
        }
    }
}