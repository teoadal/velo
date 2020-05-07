using System.IO;
using System.Reflection;
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

        private readonly string _propertyName;

        private ComponentsConverter(
            PropertyInfo property,
            IConvertersCollection converters,
            IComponentFactory componentFactory)
        {
            _converters = converters;
            _componentFactory = componentFactory;
            _propertyName = property.Name;
        }

        public object? ReadValue(JsonObject source)
        {
            var componentsData = (JsonObject) source[_propertyName];

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

        void IPropertyConverter<IEntity>.Deserialize(JsonTokenizer _, IEntity entity) => throw Error.NotSupported();
        void IPropertyConverter<IEntity>.Read(JsonObject _, IEntity entity) => throw Error.NotSupported();
        void IPropertyConverter<IEntity>.Write(IEntity entity, JsonObject _) => throw Error.NotSupported();
    }
}