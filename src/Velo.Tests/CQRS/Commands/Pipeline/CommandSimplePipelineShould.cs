using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.CQRS.Commands;
using Velo.CQRS.Commands.Pipeline;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;

namespace Velo.Tests.CQRS.Commands.Pipeline
{
    public class CommandSimplePipelineShould : CQRSTestClass
    {
        private readonly Command _command;

        private readonly ICommandPipeline<Command> _pipeline;
        private readonly Mock<ICommandProcessor<Command>> _processor;

        public CommandSimplePipelineShould()
        {
            _command = Fixture.Create<Command>();

            _processor = MockCommandProcessor(_command);
            _pipeline = new CommandSimplePipeline<Command>(_processor.Object);
        }
        
        [Fact]
        public void DisposeAfterExecute()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, CancellationToken))
                .Should().NotThrow();

            _pipeline
                .Invoking(pipeline => pipeline.Dispose())
                .Should().NotThrow();
        }

        [Fact]
        public void Execute()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, CancellationToken))
                .Should().NotThrow();
        }

        [Theory, AutoData]
        public void ExecuteManyTimes(int count)
        {
            count = EnsureValid(count);

            for (var i = 0; i < count; i++)
            {
                _pipeline
                    .Awaiting(pipeline => pipeline.Execute(_command, CancellationToken))
                    .Should().NotThrow();
            }

            EnsurePipelineExecuted(count);
        }

        [Theory, AutoData]
        public void ExecuteManyTimesParallel(int count)
        {
            count = EnsureValid(count);

            Parallel.For(0, count, _ =>
            {
                // ReSharper disable once AccessToDisposedClosure
                _pipeline
                    .Awaiting(pipeline => pipeline.Execute(_command, CancellationToken))
                    .Should().NotThrow();
            });

            EnsurePipelineExecuted(count);
        }

        [Fact]
        public void ExecuteByInterface()
        {
            var pipeline = (ICommandPipeline) _pipeline;

            pipeline
                .Awaiting(p => p.Send(_command, CancellationToken))
                .Should().NotThrow();
            
            EnsurePipelineExecuted();
        }

        [Fact]
        public void ProcessCommand()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, CancellationToken))
                .Should().NotThrow();

            EnsurePipelineExecuted();
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _pipeline.Dispose();

            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, CancellationToken))
                .Should().Throw<ObjectDisposedException>();
        }

        private void EnsurePipelineExecuted(int count = 1)
        {
            _processor
                .Verify(processor => processor
                    .Process(_command, CancellationToken), Times.Exactly(count));
        }
    }
}