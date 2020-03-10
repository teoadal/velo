using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Velo.CQRS.Commands;
using Velo.CQRS.Commands.Pipeline;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Commands
{
    public class CommandSimplePipelineShould : CQRSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly Command _command;

        private readonly CommandSimplePipeline<Command> _pipeline;
        private readonly Mock<ICommandProcessor<Command>> _processor;

        public CommandSimplePipelineShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _command = new Command();

            _processor = new Mock<ICommandProcessor<Command>>();
            _processor.Setup(processor => processor.Process(_command, _ct))
                .Returns(Task.CompletedTask);

            _pipeline = new CommandSimplePipeline<Command>(_processor.Object);
        }

        [Fact]
        public void Execute()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void ExecuteByInterface()
        {
            var pipeline = (ICommandPipeline) _pipeline;

            pipeline
                .Awaiting(p => p.Send(_command, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void ProcessCommand()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().NotThrow();

            _processor.Verify(processor => processor.Process(_command, _ct));
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _pipeline.Dispose();

            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().Throw<NullReferenceException>();
        }
    }
}