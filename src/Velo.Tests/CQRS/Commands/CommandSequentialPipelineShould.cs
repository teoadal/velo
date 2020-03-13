using System;
using System.Linq;
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
    public class CommandSequentialPipelineShould : CQRSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly Command _command;

        private readonly CommandSequentialPipeline<Command> _pipeline;

        private readonly Mock<ICommandPreProcessor<Command>> _preProcessor;
        private readonly Mock<ICommandProcessor<Command>> _processor;
        private readonly Mock<ICommandPostProcessor<Command>> _postProcessor;

        public CommandSequentialPipelineShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _command = new Command();

            _preProcessor = MockCommandPreProcessor(_command, _ct);

            _processor = new Mock<ICommandProcessor<Command>>();
            _processor
                .Setup(processor => processor.Process(_command, _ct))
                .Returns(Task.CompletedTask);

            _postProcessor = MockCommandPostProcessor(_command, _ct);

            _pipeline = new CommandSequentialPipeline<Command>(
                new[] {_preProcessor.Object},
                _processor.Object,
                new[] {_postProcessor.Object});
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
            pipeline.Awaiting(p => p.Send(_command, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void PreProcess()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().NotThrow();

            _preProcessor.Verify(processor => processor.PreProcess(_command, _ct));
        }

        [Fact]
        public void Process()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().NotThrow();

            _processor.Verify(processor => processor.Process(_command, _ct));
        }

        [Fact]
        public void PostProcess()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().NotThrow();

            _postProcessor.Verify(processor => processor.PostProcess(_command, _ct));
        }

        [Fact]
        public void UseManyPreProcessor()
        {
            var preProcessors = Many(5, () => MockCommandPreProcessor(_command, _ct));
            var pipeline = new CommandSequentialPipeline<Command>(
                preProcessors.Select(mock => mock.Object).ToArray(),
                _processor.Object,
                new[] {_postProcessor.Object});

            pipeline
                .Awaiting(pipe => pipe.Execute(_command, _ct))
                .Should().NotThrow();

            foreach (var preProcessor in preProcessors)
            {
                preProcessor.Verify(processor => processor.PreProcess(_command, _ct));
            }
        }

        [Fact]
        public void UseManyPostProcessor()
        {
            var postProcessors = Many(5, () => MockCommandPostProcessor(_command, _ct));
            var pipeline = new CommandSequentialPipeline<Command>(
                new[] {_preProcessor.Object},
                _processor.Object,
                postProcessors.Select(mock => mock.Object).ToArray());

            pipeline
                .Awaiting(pipe => pipe.Execute(_command, _ct))
                .Should().NotThrow();

            foreach (var postProcessor in postProcessors)
            {
                postProcessor.Verify(processor => processor.PostProcess(_command, _ct));
            }
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