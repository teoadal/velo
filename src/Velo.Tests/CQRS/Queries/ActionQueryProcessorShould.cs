using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using Velo.CQRS.Queries;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;

namespace Velo.Tests.CQRS.Queries
{
    public class ActionQueryProcessorShould : CQRSTestClass
    {
        private readonly Mock<Func<Query, Boo>> _action;
        private readonly Query _query;
        private readonly ActionQueryProcessor<Query, Boo> _processor;

        public ActionQueryProcessorShould()
        {
            _action = new Mock<Func<Query, Boo>>();
            _query = Fixture.Create<Query>();
            _processor = new ActionQueryProcessor<Query, Boo>(_action.Object);
        }

        [Fact]
        public void ProcessQuery()
        {
            _processor
                .Awaiting(processor => processor.Process(_query, CancellationToken))
                .Should().NotThrow();

            _action.Verify(func => func.Invoke(_query));
        }
        
        [Fact]
        public void ThrowIfDisposed()
        {
            _processor.Dispose();

            _processor
                .Awaiting(processor => processor.Process(_query, CancellationToken))
                .Should().Throw<ObjectDisposedException>();
        }
    }
}