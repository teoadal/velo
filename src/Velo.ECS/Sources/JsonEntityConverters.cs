using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Velo.Collections;
using Velo.ECS.Actors;
using Velo.ECS.Assets;
using Velo.ECS.Components;
using Velo.Serialization;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Sources
{
    internal sealed class JsonEntityConverters
    {
        private readonly Description[] _actors;
        private readonly Description[] _assets;
        private readonly Description[] _components;

        private readonly IConvertersCollection _converters;

        private readonly Func<int, IComponent[], Actor> _defaultActorBuilder;
        private readonly Func<int, IComponent[], Asset> _defaultAssetBuilder;

        public JsonEntityConverters(IConvertersCollection? converters)
        {
            _converters = converters ?? new ConvertersCollection(CultureInfo.InvariantCulture);

            var actors = new List<Description>();
            var assets = new List<Description>();
            var components = new List<Description>();

            var assemblies = ReflectionUtils.GetUserAssemblies();
            foreach (var type in assemblies.SelectMany(assembly => assembly.DefinedTypes))
            {
                if (type.IsAbstract || type.IsInterface) continue;

                if (typeof(Actor).IsAssignableFrom(type))
                {
                    actors.Add(CreateDescription(type, "Actor"));
                }
                else if (typeof(Asset).IsAssignableFrom(type))
                {
                    assets.Add(CreateDescription(type, "Asset"));
                }
                else if (typeof(IComponent).IsAssignableFrom(type))
                {
                    components.Add(CreateDescription(type, "Component"));
                }
            }

            _actors = actors.ToArray();
            _assets = assets.ToArray();
            _components = components.ToArray();

            _defaultActorBuilder = (id, actorComponents) => new Actor(id, actorComponents);
            _defaultAssetBuilder = (id, actorComponents) => new Asset(id, actorComponents);
        }

        public Actor DeserializeActor(ref JsonTokenizer tokenizer)
        {
            return VisitEntity(ref tokenizer, _actors, _defaultActorBuilder);
        }

        public Asset DeserializeAsset(ref JsonTokenizer tokenizer)
        {
            return VisitEntity(ref tokenizer, _assets, _defaultAssetBuilder);
        }

        private Description CreateDescription(Type type, string replace)
        {
            var converter = (IObjectConverter) _converters.Get(type);
            var name = type.Name.Replace(replace, string.Empty);

            return new Description(converter, name, type);
        }

        private IComponent DeserializeComponent(string componentName, ref JsonTokenizer tokenizer)
        {
            return TryGetDescription(_components, componentName, out var description)
                ? (IComponent) description.Converter.DeserializeObject(ref tokenizer)
                : throw Error.NotFound($"Component implementation with name '{componentName}' not found");
        }

        private static bool TryGetDescription(Description[] descriptions, string name, out Description description)
        {
            var comparer = StringUtils.IgnoreCaseComparer;
            foreach (var exists in descriptions)
            {
                if (!comparer.Equals(exists.Name, name)) continue;

                description = exists;
                return true;
            }

            description = default;
            return false;
        }

        private IComponent[] VisitComponents(ref JsonTokenizer tokenizer)
        {
            var result = new LocalList<IComponent>();

            tokenizer.MoveNext(); // skip object start

            while (tokenizer.MoveNext())
            {
                var token = tokenizer.Current;

                if (token.TokenType == JsonTokenType.ObjectEnd) break;

                var componentName = token.GetNotNullPropertyName();

                tokenizer.MoveNext(); // to property value;

                var component = DeserializeComponent(componentName, ref tokenizer);

                result.Add(component);
            }

            return result.ToArray();
        }

        private T VisitEntity<T>(ref JsonTokenizer tokenizer,
            Description[] descriptions,
            Func<int, IComponent[], T> builder)
        {
            var entityId = int.MinValue;
            string? entityType = null;
            IComponent[] components = Array.Empty<IComponent>();
            var otherProperties = new JsonObject();

            while (tokenizer.MoveNext())
            {
                var token = tokenizer.Current;

                if (token.TokenType == JsonTokenType.ObjectEnd) break;

                var propertyName = token.GetNotNullPropertyName();
                switch (propertyName)
                {
                    case "id":
                        entityId = VisitKnownProperty<int>(ref tokenizer);
                        break;
                    case "components":
                        components = VisitComponents(ref tokenizer);
                        break;
                    case "_type":
                        entityType = VisitKnownProperty<string>(ref tokenizer);
                        break;
                    default:
                        var propertyValue = JsonVisitor.VisitProperty(ref tokenizer);
                        otherProperties.Add(propertyName, propertyValue);
                        break;
                }
            }

            if (entityType == null) return builder(entityId, components);

            if (!TryGetDescription(descriptions, entityType, out var description))
            {
                throw Error.NotFound($"Entity implementation with name '{entityType}' not found");
            }
                
            var instance = Activator.CreateInstance(description.Type, entityId, components);
            description.Converter.FillObject(otherProperties, instance);
            
            return (T) instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TValue VisitKnownProperty<TValue>(ref JsonTokenizer tokenizer)
        {
            var propertyValue = JsonVisitor.VisitProperty(ref tokenizer);
            return _converters.Read<TValue>(propertyValue);
        }

        private readonly struct Description
        {
            public readonly IObjectConverter Converter;
            public readonly string Name;
            public readonly Type Type;

            public Description(IObjectConverter converter, string name, Type type)
            {
                Converter = converter;
                Name = name;
                Type = type;
            }
        }
    }
}