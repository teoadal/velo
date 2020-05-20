using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using Velo.CQRS.Commands;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;

namespace Velo.Tests.CQRS.Commands
{
    public class ActionCommandProcessorShould : CQRSTestClass
    {
        private readonly Mock<Action<Command>> _action;
        private readonly Command _command;
        private readonly ActionCommandProcessor<Command> _processor;

        public ActionCommandProcessorShould()
        {
            _action = new Mock<Action<Command>>();
            _command = Fixture.Create<Command>();
            _processor = new ActionCommandProcessor<Command>(_action.Object);
        }

        [Fact]
        public void ProcessCommand()
        {
            _processor
                .Awaiting(processor => processor.Process(_command, CancellationToken))
                .Should().NotThrow();

            _action.Verify(action => action.Invoke(_command));
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _processor.Dispose();

            _processor
                .Awaiting(processor => processor.Process(_command, CancellationToken))
                .Should().Throw<ObjectDisposedException>();
        }
    }
}