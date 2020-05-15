using System;
using System.Collections.Generic;
using System.Linq;
using Velo.ECS.Components;
using Velo.Text;
using Velo.Utils;

namespace Velo.ECS.Sources
{
    internal sealed class SourceDescriptions
    {
        private readonly Description[] _components;
        private readonly Description[] _entities;

        private readonly Dictionary<int, string> _aliases;

        public SourceDescriptions()
        {
            var components = new List<Description>();
            var entities = new List<Description>();
            foreach (var assembly in ReflectionUtils.GetUserAssemblies())
            {
                foreach (var type in assembly.DefinedTypes)
                {
                    if (type.IsAbstract || type.IsInterface) continue;

                    if (typeof(IEntity).IsAssignableFrom(type))
                    {
                        entities.Add(new Description(type));
                    }
                    else if (typeof(IComponent).IsAssignableFrom(type))
                    {
                        components.Add(new Description(type));
                    }
                }
            }

            _components = components.ToArray();
            _entities = entities.ToArray();

            _aliases = new Dictionary<int, string>();
        }

        public static string BuildTypeName(Type type)
        {
            return type.Name.Cut("Component");
        }

        public string GetComponentName(Type componentType)
        {
            return TryGetDescription(_components, componentType, out var description)
                ? description.Name
                : throw Error.NotFound($"Description for '{ReflectionUtils.GetName(componentType)}' isn't found");
        }

        public Type GetComponentType(string name)
        {
            return TryGetDescription(_components, name, out var description)
                ? description.Type
                : throw Error.NotFound($"Implementation type for component with name '{name}' isn't found");
        }

        public Type GetEntityType(string name)
        {
            return TryGetDescription(_entities, name, out var description)
                ? description.Type
                : throw Error.NotFound($"Implementation type for entity with name '{name}' isn't found");
        }

        public int GetOrAddAlias(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                throw Error.InvalidOperation("Alias can't be null or whitespace");
            }

            var id = alias.Sum(ch => ch.GetHashCode());

            if (!_aliases.ContainsKey(id))
            {
                _aliases.Add(id, alias);
            }

            return id;
        }

        public bool TryGetAlias(int entityId, out string alias)
        {
            return _aliases.TryGetValue(entityId, out alias);
        }

        private bool TryGetDescription(Description[] descriptions, string name, out Description description)
        {
            var comparer = StringUtils.IgnoreCaseComparer;

            foreach (var element in descriptions)
            {
                if (!comparer.Equals(element.Name, name)) continue;

                description = element;
                return true;
            }

            description = default;
            return false;
        }

        private bool TryGetDescription(Description[] descriptions, Type type, out Description description)
        {
            foreach (var element in descriptions)
            {
                if (element.Type != type) continue;

                description = element;
                return true;
            }

            description = default;
            return false;
        }

        private readonly struct Description
        {
            public readonly string Name;
            public readonly Type Type;

            public Description(Type type)
            {
                Name = BuildTypeName(type);
                Type = type;
            }
        }
    }
}