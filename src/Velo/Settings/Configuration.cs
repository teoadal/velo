using System;
using System.Collections.Generic;
using System.Globalization;
using Velo.Extensions;
using Velo.Serialization;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;
using Velo.Settings.Sources;
using Velo.Utils;

namespace Velo.Settings
{
    internal sealed class Configuration : IConfiguration
    {
        public IConfigurationSource[] Sources => _sources;

        private readonly Dictionary<string, object> _cache;
        private readonly IConvertersCollection _converters;
        private readonly IConfigurationSource[] _sources;

        private JsonObject _configuration;

        public Configuration(IConvertersCollection converters = null, IConfigurationSource[] sources = null)
        {
            if (sources == null) sources = Array.Empty<IConfigurationSource>();

            _cache = new Dictionary<string, object>();
            _converters = converters ?? new ConvertersCollection(CultureInfo.InvariantCulture);
            _sources = sources;

            Reload();
        }

        public bool Contains(string path)
        {
            return TryGetJsonData(path, out _);
        }

        public string Get(string path)
        {
            if (!TryGetJsonData(path, out var jsonData))
            {
                throw PathNotFound(path);
            }

            var jsonDataType = jsonData.Type;
            if (jsonDataType == JsonDataType.Array || jsonDataType == JsonDataType.Object)
            {
                throw CastException(jsonDataType, "string");
            }

            var value = (JsonValue) jsonData;
            return value.Value;
        }

        public T Get<T>(string path)
        {
            if (TryGet<T>(path, out var value)) return value;
            throw PathNotFound(path);
        }

        public bool TryGet<T>(string path, out T value)
        {
            if (_cache.TryGetValue(path, out var cached))
            {
                value = (T) cached;
                return true;
            }
            
            if (!TryGetJsonData(path, out var jsonData))
            {
                value = default;
                return false;
            }

            var converter = GetValidConverter<T>(jsonData);
            value = converter.Read(jsonData);
            
            using (Lock.Enter(_cache))
            {
                _cache[path] = value;
            }
            
            return true;
        }

        public void Reload()
        {
            if (_sources == null) return;

            var root = new JsonObject();
            foreach (var source in _sources)
            {
                if (source.TryGet(out var sourceData))
                {
                    CopyValues(sourceData, root);
                }
            }

            using (Lock.Enter(_cache))
            {
                _cache.Clear();
                _configuration = root;
            }
        }

        private static void CopyValues(JsonObject from, JsonObject to)
        {
            foreach (var (property, value) in from)
            {
                if (value.Type == JsonDataType.Object && to.TryGet(property, out var targetValue))
                {
                    CopyValues((JsonObject) targetValue, (JsonObject) value);
                }
                else
                {
                    to[property] = value;
                }
            }
        }

        private IJsonConverter<T> GetValidConverter<T>(JsonData data)
        {
            var jsonDataType = data.Type;
            var primitiveData = jsonDataType != JsonDataType.Object && jsonDataType != JsonDataType.Array;
            
            var converter = _converters.Get<T>();
            if (converter.IsPrimitive && !primitiveData || !converter.IsPrimitive && primitiveData)
            {
                throw CastException(jsonDataType, ReflectionUtils.GetName<T>());
            }

            return converter;
        }

        private bool TryGetJsonData(string path, out JsonData data)
        {
            var properties = path.Split('.');

            var instance = _configuration;

            data = null;
            foreach (var property in properties)
            {
                if (data != null) instance = (JsonObject) data;
                if (instance.TryGet(property, out data)) continue;

                data = null;
                return false;
            }

            return true;
        }

        private static InvalidCastException CastException(JsonDataType dataType, string targetTypeName)
        {
            return Error.Cast($"Can't cast '{dataType}' to '{targetTypeName}'");
        }

        private static KeyNotFoundException PathNotFound(string path)
        {
            return Error.NotFound($"Configuration path '{path}' not found");
        }
    }
}