using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using Velo.Collections.Local;
using Velo.DependencyInjection;
using Velo.ECS.Assets;
using Velo.ECS.Sources;
using Velo.ECS.Sources.Context;
using Velo.ECS.Sources.Json;
using Velo.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Sources.Json
{
    public class JsonFileSourceShould : ECSTestClass
    {
        private readonly Asset[] _assets;
        private readonly IEntitySource<Asset> _source;
        private readonly IEntitySourceContext<Asset> _sourceContext;

        public JsonFileSourceShould(ITestOutputHelper output) : base(output)
        {
            var provider = new DependencyCollection()
                .AddECS()
                .BuildProvider();

            _assets = Many(10, i => CreateAsset(i));

            var serialized = provider.GetRequiredService<JConverter>().Serialize(_assets);

            var fileName = $"{nameof(JsonFileSourceShould)}.json";
            File.WriteAllText(fileName, serialized);

            _source = provider.Activate<JsonFileSource<Asset>>(new LocalList<object>(fileName));
            _sourceContext = Mock.Of<IEntitySourceContext<Asset>>();
        }

        [Fact]
        public void ReadEntities()
        {
            var actual = _source.GetEntities(_sourceContext).ToArray();

            // ReSharper disable once CoVariantArrayConversion
            actual.Should().BeEquivalentTo(_assets);
        }

        [Fact]
        public void DisposeNotAffect()
        {
            _source
                .Invoking(source => source.GetEntities(_sourceContext).ToArray())
                .Should().NotThrow();

            _source
                .Invoking(source => source.Dispose())
                .Should().NotThrow();
        }
    }
}