using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.CQRS;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Queries
{
    public class ActionQueryProcessorShould : TestClass
    {
        private readonly DependencyCollection _dependencyCollection;

        public ActionQueryProcessorShould(ITestOutputHelper output) : base(output)
        {
            _dependencyCollection = new DependencyCollection()
                .AddEmitter();
        }

        [Theory]
        [AutoData]
        public void Executed(Query query)
        {
            var processor = new Mock<Func<Query, Boo>>();

            var emitter = _dependencyCollection
                .CreateQueryProcessor(processor.Object)
                .BuildProvider()
                .GetRequiredService<IEmitter>();

            emitter.Awaiting(e => e.Ask(query)).Should().NotThrow();

            processor.Verify(p => p.Invoke(query));
        }

        [Theory]
        [AutoData]
        public void ExecutedWithContext(Query query)
        {
            var repository = Mock.Of<IBooRepository>();
            var processor = new Mock<Func<Query, IBooRepository, Boo>>();

            var emitter = _dependencyCollection
                .AddInstance(repository)
                .CreateQueryProcessor(processor.Object)
                .BuildProvider()
                .GetRequiredService<IEmitter>();

            emitter.Awaiting(e => e.Ask(query)).Should().NotThrow();

            processor.Verify(p => p.Invoke(query, repository));
        }
    }
}