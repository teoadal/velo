using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Velo.ECS.Assets;
using Velo.ECS.Sources;
using Velo.ECS.Sources.Context;
using Xunit;

namespace Velo.Tests.ECS.Sources
{
    public class DelegateSourceShould : ECSTestClass
    {
        private Mock<Func<IEntitySourceContext<Asset>, IEnumerable<Asset>>> _builder;
        private readonly IEntitySourceContext<Asset> _context;
        private readonly DelegateSource<Asset> _source;

        public DelegateSourceShould()
        {
            _context = Mock.Of<IEntitySourceContext<Asset>>();

            _builder = new Mock<Func<IEntitySourceContext<Asset>, IEnumerable<Asset>>>();
            _builder
                .Setup(b => b.Invoke(_context))
                .Returns(Enumerable.Empty<Asset>());

            _source = new DelegateSource<Asset>(_builder.Object);
        }

        [Fact]
        public void CallDelegate()
        {
            _source
                .Invoking(source => source.GetEntities(_context))
                .Should().NotThrow()
                .Which.Should().NotBeNull();

            _builder.Verify(builder => builder.Invoke(_context));
        }

        [Fact]
        public void DisposeNotAffect()
        {
            _source
                .Invoking(source => source.Dispose())
                .Should().NotThrow();
        }
    }
}