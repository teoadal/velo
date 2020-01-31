using System.Collections.Generic;

namespace Velo.Serialization.Models
{
    internal sealed class JsonObject : JsonData
    {
        private readonly Dictionary<string, JsonData> _properties;

        public JsonObject() : base(JsonDataType.Object)
        {
            _properties = new Dictionary<string, JsonData>();
        }

        public void Add(string property, JsonData value)
        {
            _properties.Add(string.Intern(property), value);
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