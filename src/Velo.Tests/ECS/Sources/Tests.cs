using System.IO;
using System.Linq;
using System.Text;
using AutoFixture;
using FluentAssertions;
using Velo.Collections.Local;
using Velo.DependencyInjection;
using Velo.ECS.Assets;
using Velo.ECS.Assets.Context;
using Velo.ECS.Components;
using Velo.Serialization;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Sources
{
    public class Tests : ECSTestClass
    {
        private readonly Asset[] _assets;
        private readonly TestAsset _asset;

        private readonly IJsonConverter _converter;
        private readonly JConverter _jsonConverter;

        public Tests(ITestOutputHelper output) : base(output)
        {
            _jsonConverter = new DependencyCollection()
                .AddECS()
                .BuildProvider()
                .GetRequiredService<JConverter>();
            _converter = _jsonConverter.Converters.Get(typeof(TestAsset));

            var reference = CreateAsset(6);
            var assets = new LocalList<Asset>(
                new TestAsset(7, new IComponent[] {Fixture.Create<TestComponent1>()})
                {
                    Array = Fixture.CreateMany<int>().ToArray(),
                    Reference = reference
                },
                CreateAsset(4, Fixture.Create<TestComponent1>()),
                CreateAsset(3, Fixture.Create<TestComponent2>()),
                CreateAsset(2),
                reference);

            var components = new IComponent[]
            {
                Fixture.Create<TestComponent1>(),
                Fixture.Create<TestComponent2>(),
                new TestComponent3
                {
                    Asset = (TestAsset) assets[0],
                    AssetsArray = assets.ToArray(),
                    AssetsList = assets.ToList(),
                }
            };
            _asset = new TestAsset(1, components) {Array = Fixture.CreateMany<int>().ToArray()};

            assets.Add(_asset);
            assets.Reverse();
            _assets = assets.ToArray();
        }

        [Fact]
        public void Serialize()
        {
            var writer = new StringWriter();
            _converter.SerializeObject(_asset, writer);
            var serialized = writer.ToString();

            serialized.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CorrectConverter2()
        {
            var serialized = _jsonConverter.Serialize(_assets);
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));

            var provider = new DependencyCollection()
                .AddECS()
                .AddJsonStreamAssets(memoryStream)
                .BuildProvider();

            var assets = provider.GetRequiredService<IAssetContext>();

            var actual = (TestAsset) assets.Get(_asset.Id);

            actual.Id.Should().Be(_asset.Id);
            actual.Reference.Should().BeEquivalentTo(_asset.Reference);
            actual.Components.Should().NotBeNullOrEmpty();

            var actualComponents = actual.Components.ToArray();
            var expectedComponents = _asset.Components.ToArray();

            CompareComponents<TestComponent1>(actualComponents[0], expectedComponents[0]);
            CompareComponents<TestComponent2>(actualComponents[1], expectedComponents[1]);
            CompareComponents<TestComponent3>(actualComponents[2], expectedComponents[2]);
        }
    }
}