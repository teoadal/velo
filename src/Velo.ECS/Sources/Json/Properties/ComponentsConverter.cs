using System.IO;
using Velo.Collections.Local;
using Velo.ECS.Components;
using Velo.Extensions;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.Serialization.Objects;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Sources.Json.Properties
{
    internal sealed class ComponentsConverter : IPropertyConverter<IEntity>
    {
        private readonly IConvertersCollection _converters;
        private readonly IComponentFactory _componentFactory;

        public ComponentsConverter(IConvertersCollection converters, IComponentFactory componentFactory)
        {
            _converters = converters;
            _componentFactory = componentFactory;
        }

        public object? ReadValue(JsonObject source)
        {
            var componentsData = (JsonObject) source[nameof(IEntity.Components)];

            var components = new LocalList<IComponent>();
            foreach (var (name, data) in componentsData)
            {
                var componentType = SourceDescriptions.GetComponentType(name);

                var component = _componentFactory.Create(componentType);

                var componentConverter = (IObjectConverter) _converters.Get(componentType);
                var filledComponent = componentConverter.FillObject((JsonObject) data, component);

                if (filledComponent != null)
                {
                    components.Add((IComponent) filledComponent);
                }
            }

            return components.ToArray();
        }

        public void Serialize(IEntity instance, TextWriter output)
        {
            output.Write('{');

            var first = true;
            foreach (var component in instance.Components)
            {
                if (first) first = false;
                else output.Write(',');

                var componentType = component.GetType();
                var componentName = SourceDescriptions.GetComponentName(componentType);
                var componentConverter = _converters.Get(componentType);

                output.WriteProperty(componentName);
                componentConverter.SerializeObject(component, output);
            }

            output.Write('}');
        }

        public void Write(IEntity instance, JsonObject output)
        {
            var componentsData = new JsonObject();

            foreach (var component in instance.Components)
            {
                var componentType = component.GetType();
                var componentName = SourceDescriptions.GetComponentName(componentType);
                var componentConverter = _converters.Get(componentType);

                var componentData = componentConverter.WriteObject(component);
                componentsData.Add(componentName, componentData);
            }
            
            output.Add(nameof(IEntity.Components), componentsData);
        }

        void IPropertyConverter<IEntity>.Deserialize(JsonTokenizer _, IEntity entity) => throw Error.NotSupported();
        void IPropertyConverter<IEntity>.Read(JsonObject _, IEntity entity) => throw Error.NotSupported();
    }
}