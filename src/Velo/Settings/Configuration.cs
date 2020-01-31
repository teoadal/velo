using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
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
        private readonly ConvertersCollection _converters;
        private readonly IConfigurationSource[] _sources;

        private JsonObject _configuration;

        public Configuration(IConfigurationSource[] sources = null)
        {
            if (sources == null) sources = Array.Empty<IConfigurationSource>();

            _cache = new Dictionary<string, object>();
            _converters = new ConvertersCollection(CultureInfo.InvariantCulture);
            _sources = sources;

            Reload();
        }

        public string Get(string path)
        {
            var value = (JsonValue) GetJsonData(path);

            return value?.Value;
        }

        public T Get<T>(string path)
        {
            var lockTaken = false;

            try
            {
                Monitor.Enter(_cache, ref lockTaken);

                if (_cache.TryGetValue(path, out var cached)) return (T) cached;

                var data = GetJsonData(path);
                var primitiveData = data.Type != JsonDataType.Object && data.Type != JsonDataType.Array;

                var converter = (IJsonConverter<T>) _converters.Get(Typeof<T>.Raw);

                if (converter.IsPrimitive && !primitiveData || !converter.IsPrimitive && primitiveData)
                {
                    throw Error.Cast($"Can't cast '{data.Type}' to {ReflectionUtils.GetName<T>()}");
                }

                var section = converter.Read(data);
                _cache.Add(path, section);
                return section;
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_cache);
            }
        }

        public void Reload()
        {
            if (_sources == null) return;

            var lockTaken = false;
            try
            {
                Monitor.Enter(_cache, ref lockTaken);

                _cache.Clear();

                var root = new JsonObject();

                foreach (var source in _sources)
                {
                    var sourceData = source.FetchData();
                    if (sourceData == null) continue;

                    CopyValues(sourceData, root);
                }

                _configuration = root;
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_cache);
            }
        }

        private JsonData GetJsonData(string path)
        {
            var properties = path.Split('.');

            var instance = _configuration;
            JsonData data = null;
            foreach (var property in properties)
            {
                if (data != null) instance = (JsonObject) data;

                if (!instance.TryGet(property, out data))
                {
                    throw Error.NotFound($"Configuration path '{path}' not found");
                }
            }

            return data;
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
    }
}