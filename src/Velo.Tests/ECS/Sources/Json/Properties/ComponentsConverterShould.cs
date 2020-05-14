using System.IO;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Velo.DependencyInjection;
using Velo.ECS;
using Velo.ECS.Components;
using Velo.ECS.Sources;
using Velo.ECS.Sources.Json.Properties;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Sources.Json.Properties
{
    public class ComponentsConverterShould : ECSTestClass
    {
        private readonly TestAsset _asset;
        private readonly JsonObject _assetData;

        private readonly string _component1Name;
        private readonly string _component2Name;

        private readonly ComponentsConverter _converter;

        public ComponentsConverterShould(ITestOutputHelper output) : base(output)
        {
            var provider = new DependencyCollection()
                .AddECS()
                .BuildProvider();

            var components = new IComponent[] {Fixture.Create<TestComponent1>(), Fixture.Create<TestComponent2>()};
            _asset = new TestAsset(1, components) {Reference = CreateAsset(2)};

            var converters = provider.GetRequiredService<IConvertersCollection>();
            _converter = new ComponentsConverter(converters, provider.GetRequiredService<IComponentFactory>());

            _assetData = (JsonObject) converters.Write(_asset);

            _component1Name = SourceDescriptions.BuildTypeName(typeof(TestComponent1));
            _component2Name = SourceDescriptions.BuildTypeName(typeof(TestComponent2));
        }

        [Fact]
        public void ReadValue()
        {
            var actual = (IComponent[]) _converter.ReadValue(_assetData);
            var expected = _asset.Components.ToArray();

            CompareComponents<TestComponent1>(actual![0], expected[0]);
        }

        [Fact]
        public void Serialize()
        {
            var writer = new StringWriter();

            _converter.Serialize(_asset, writer);

            var components = _asset.Components.ToArray();

            var actual = writer.ToString();
            actual
                .Should().Contain(_component1Name)
                .And.Contain(_component2Name)
                .And.Contain(((TestComponent1) components[0]).Int.ToString())
                .And.Contain(((TestComponent2) components[1]).String);
        }

        [Fact]
        public void Write()
        {
            var output = new JsonObject();

            _converter.Write(_asset, output);

            output.TryGet(nameof(IEntity.Components), out var components).Should().BeTrue();

            var componentsData = (JsonObject) components;

            componentsData.Contains(_component1Name).Should().BeTrue();
            componentsData.Contains(_component2Name).Should().BeTrue();
        }
    }
}