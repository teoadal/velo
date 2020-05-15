using System.IO;
using AutoFixture;
using FluentAssertions;
using Velo.DependencyInjection;
using Velo.ECS;
using Velo.ECS.Components;
using Velo.ECS.Sources.Json.Properties;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Sources.Json.Properties
{
    public sealed class IdConverterShould : ECSTestClass
    {
        private readonly TestAsset _asset;
        private readonly JsonObject _assetData;

        private readonly IdConverter _converter;

        public IdConverterShould(ITestOutputHelper output) : base(output)
        {
            var provider = new DependencyCollection()
                .AddECS()
                .BuildProvider();

            var components = new IComponent[] {Fixture.Create<TestComponent1>(), Fixture.Create<TestComponent2>()};
            _asset = new TestAsset(1, components) {Reference = CreateAsset(2)};

            var converters = provider.GetRequiredService<IConvertersCollection>();
            _converter = provider.Activate<IdConverter>();

            _assetData = (JsonObject) converters.Write(_asset);
        }

        [Fact]
        public void ReadValue()
        {
            var id = (int) _converter.ReadValue(_assetData)!;
            id.Should().Be(_asset.Id);
        }

        [Fact]
        public void Serialize()
        {
            var writer = new StringWriter();

            _converter.Serialize(_asset, writer);

            writer.ToString().Should().Contain(_asset.Id.ToString());
        }

        [Fact]
        public void Write()
        {
            var output = new JsonObject();

            _converter.Write(_asset, output);

            output.TryGet(nameof(IEntity.Id), out var id).Should().BeTrue();

            var idValue = (JsonValue) id;
            idValue.Type.Should().Be(JsonDataType.Number);
            idValue.Value.Should().Be(_asset.Id.ToString());
        }
    }
}