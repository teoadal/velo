using System;
using System.Collections.Generic;
using Velo.Collections;
using Velo.Extensions;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.Settings.Sources;
using Velo.Utils;

namespace Velo.Settings.Provider
{
    internal sealed class SettingsProvider : DangerousVector<string, object>, ISettingsProvider
    {
        public ISettingsSource[] Sources => _sources;

        private readonly IConvertersCollection _converters;
        private readonly Func<string, Type, object> _sectionBuilder;
        private readonly ISettingsSource[] _sources;

        private JsonObject _configuration;

        public SettingsProvider(ISettingsSource[] sources, IConvertersCollection converters)
        {
            _configuration = new JsonObject();
            _converters = converters;
            _sectionBuilder = BuildSection;
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
            value = (T) GetOrAdd(path, _sectionBuilder, Typeof<T>.Raw);
            return value != null;
        }

        public void Reload()
        {
            var root = new JsonObject();
            foreach (var source in _sources)
            {
                if (source.TryGet(out var sourceData))
                {
                    CopyValues(sourceData, root);
                }
            }

            ClearSafe();

            _configuration = root;
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

        private object BuildSection(string path, Type sectionType)
        {
            if (!TryGetJsonData(path, out var jsonData)) return null!;

            var jsonDataType = jsonData.Type;
            var primitiveData = jsonDataType != JsonDataType.Object && jsonDataType != JsonDataType.Array;

            var converter = _converters.Get(sectionType);
            if (converter.IsPrimitive && !primitiveData || !converter.IsPrimitive && primitiveData)
            {
                throw CastException(jsonDataType, ReflectionUtils.GetName(sectionType));
            }

            var sectionData = converter.ReadObject(jsonData);
            if (sectionData == null) throw Error.Cast($"Settings section '{path}' is null");
            return sectionData;
        }

        private bool TryGetJsonData(string path, out JsonData data)
        {
            var properties = path.Split('.');

            var instance = _configuration;

            data = null!;
            foreach (var property in properties)
            {
                if (data != null) instance = (JsonObject) data;
                if (instance.TryGet(property, out data)) continue;

                data = null!;
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