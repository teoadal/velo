using System;
using System.Collections.Generic;
using Velo.Threading;
using Velo.Utils;

namespace Velo.ECS.Components
{
    internal sealed class ComponentFactory : IComponentFactory
    {
        private readonly IComponentBuilder[] _builders;
        private readonly Dictionary<int, IComponentBuilder> _resolvedBuilders;
        private readonly object _lock;

        public ComponentFactory(IComponentBuilder[]? builders = null)
        {
            _builders = builders ?? Array.Empty<IComponentBuilder>();
            _resolvedBuilders = new Dictionary<int, IComponentBuilder>();
            _lock = new object();
        }

        public IComponent Create(Type componentType)
        {
            var typeId = Typeof.GetTypeId(componentType);

            // ReSharper disable once InvertIf
            if (!_resolvedBuilders.TryGetValue(typeId, out var componentBuilder))
            {
                var builderType = typeof(IComponentBuilder<>).MakeGenericType(componentType);
                foreach (var builder in _builders)
                {
                    if (!builderType.IsAssignableFrom(builderType)) continue;

                    componentBuilder = builder;
                    break;
                }

                AddBuilder(typeId, componentBuilder ??= CreateBuilder(componentType));
            }

            return componentBuilder.BuildComponent();
        }

        public TComponent Create<TComponent>() where TComponent : IComponent
        {
            var typeId = Typeof<TComponent>.Id;

            // ReSharper disable once InvertIf
            if (!_resolvedBuilders.TryGetValue(typeId, out var componentBuilder))
            {
                foreach (var builder in _builders)
                {
                    if (!(builder is IComponentBuilder<TComponent>)) continue;

                    componentBuilder = builder;
                    break;
                }

                AddBuilder(typeId, componentBuilder ??= CreateBuilder(Typeof<TComponent>.Raw));
            }

            return ((IComponentBuilder<TComponent>) componentBuilder).Build();
        }

        private void AddBuilder(int typeId, IComponentBuilder builder)
        {
            using (Lock.Enter(_lock))
            {
                _resolvedBuilders[typeId] = builder;
            }
        }

        private static IComponentBuilder CreateBuilder(Type componentType)
        {
            if (!ReflectionUtils.HasEmptyConstructor(componentType))
            {
                throw Error.InvalidOperation(
                    $"Component {ReflectionUtils.GetName(componentType)} hasn't empty constructor - create own builder");
            }

            var builderType = typeof(DefaultComponentBuilder<>).MakeGenericType(componentType);
            return (IComponentBuilder) Activator.CreateInstance(builderType);
        }
    }
}