using System.IO;
using System.Linq;
using System.Text;
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

namespace Velo.Tests.ECS.Sources.Json
{
    public sealed class JsonStreamSourceShould : ECSTestClass
    {
        private readonly Asset[] _assets;
        private readonly IEntitySource<Asset> _source;
        private readonly IEntitySourceContext<Asset> _sourceContext;

        public JsonStreamSourceShould()
        {
            var provider = new DependencyCollection()
                .AddECS()
                .BuildProvider();

            _assets = Many(10, i => CreateAsset(i));

            var serialized = provider.GetRequired<JConverter>().Serialize(_assets);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));

            _source = provider.Activate<JsonStreamSource<Asset>>(new LocalList<object>(memoryStream));
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