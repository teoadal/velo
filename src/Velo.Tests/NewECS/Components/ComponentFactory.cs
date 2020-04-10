using System;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Tests.NewECS.Components
{
    internal sealed class ComponentFactory : IComponentFactory
    {
        private readonly IComponentBuilder[] _builders;
        private readonly Dictionary<int, IComponentBuilder> _resolvedBuilders;
        private readonly object _lock;

        public ComponentFactory(IComponentBuilder[] builders = null)
        {
            _builders = builders ?? Array.Empty<IComponentBuilder>();
            _resolvedBuilders = new Dictionary<int, IComponentBuilder>();
            _lock = new object();
        }

        public TComponent Create<TComponent>() where TComponent : IComponent
        {
            var typeId = Typeof<TComponent>.Id;

            // ReSharper disable once InvertIf
            if (!_resolvedBuilders.TryGetValue(typeId, out var componentBuilder))
            {
                componentBuilder = FindOrCreateBuilder<TComponent>();

                using (Lock.Enter(_lock))
                {
                    _resolvedBuilders[typeId] = componentBuilder;
                }
            }

            return ((IComponentBuilder<TComponent>) componentBuilder).Build();
        }

        private IComponentBuilder FindOrCreateBuilder<TComponent>() where TComponent : IComponent
        {
            foreach (var builder in _builders)
            {
                if (builder is IComponentBuilder<TComponent>) return builder;
            }

            var componentType = Typeof<TComponent>.Raw;
            if (!ReflectionUtils.HasEmptyConstructor(componentType))
            {
                throw Error.InvalidOperation(
                    $"Component {ReflectionUtils.GetName<TComponent>()} hasn't empty constructor");
            }

            var builderType = typeof(DefaultComponentBuilder<>).MakeGenericType(componentType);
            return (IComponentBuilder) Activator.CreateInstance(builderType);
        }
    }
}