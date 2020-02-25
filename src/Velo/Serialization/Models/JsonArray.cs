using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Velo.Serialization.Models
{
    internal sealed class JsonArray : JsonData, IEnumerable<JsonData>
    {
        public static readonly JsonArray Empty = new JsonArray(Array.Empty<JsonData>());

        public readonly int Length;

        private readonly JsonData[] _elements;

        public JsonArray(JsonData[] elements) : base(JsonDataType.Array)
        {
            _elements = elements;
            Length = elements.Length;
        }

        public JsonArray(IEnumerable<JsonData> elements) : base(JsonDataType.Array)
        {
            _elements = elements.ToArray();
            Length = _elements.Length;
        }

        public bool Contains(JsonData data)
        {
            foreach (var element in _elements)
            {
                if (element.Equals(data)) return true;
            }

            return false;
        }

        public IEnumerator<JsonData> GetEnumerator()
        {
            return ((IEnumerable<JsonData>) _elements).GetEnumerator();
        }

        public override void Serialize(TextWriter writer)
        {
            writer.Write('[');

            var first = true;
            foreach (var element in _elements)
            {
                if (first) first = false;
                else writer.Write(',');
                
                element.Serialize(writer);
            }
            
            writer.Write(']');
        }
        
        public ref JsonData this[int index] => ref _elements[index];

        IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();
    }
}