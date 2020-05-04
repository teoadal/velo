using System;
using System.Collections.Generic;
using Velo.ECS.Components;
using Velo.Utils;

namespace Velo.ECS.Sources
{
    internal sealed class SourceDescriptions
    {
        private readonly Description[] _components;
        private readonly Description[] _entities;

        public SourceDescriptions()
        {
            var components = new List<Description>();
            var entities = new List<Description>();
            foreach (var assembly in ReflectionUtils.GetUserAssemblies())
            {
                foreach (var type in assembly.DefinedTypes)
                {
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
        }

        public Type GetComponentType(string name)
        {
            return TryGetDescription(_components, name, out var description)
                ? description.Type
                : throw Error.NotFound($"Implementation type for component with name {name} isn't found");
        }

        public Type GetEntityType(string name)
        {
            return TryGetDescription(_entities, name, out var description)
                ? description.Type
                : throw Error.NotFound($"Implementation type for entity with name {name} isn't found");
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

        private readonly struct Description
        {
            public readonly string Name;
            public readonly Type Type;

            public Description(Type type, string? name = null)
            {
                Name = name ?? type.Name
                    .Replace("Actor", string.Empty)
                    .Replace("Asset", string.Empty)
                    .Replace("Component", string.Empty);

                Type = type;
            }
        }
    }
}