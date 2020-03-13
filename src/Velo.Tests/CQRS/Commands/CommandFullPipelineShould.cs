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
    public class CommandFullPipelineShould : CQRSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly Command _command;

        private readonly CommandFullPipeline<Command> _pipeline;

        private readonly Mock<ICommandBehaviour<Command>> _behaviour;
        private readonly Mock<ICommandPreProcessor<Command>> _preProcessor;
        private readonly Mock<ICommandProcessor<Command>> _processor;
        private readonly Mock<ICommandPostProcessor<Command>> _postProcessor;

        public CommandFullPipelineShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _command = new Command();

            _behaviour = MockBehaviour();
            _preProcessor = MockCommandPreProcessor(_command, _ct);
            _processor = MockCommandProcessor(_command, _ct);
            _postProcessor = MockCommandPostProcessor(_command, _ct);

            _pipeline = new CommandFullPipeline<Command>(
                new[] {_behaviour.Object},
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
            pipeline
                .Awaiting(p => p.Send(_command, _ct))
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
        public void UseBehaviour()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().NotThrow();

            _behaviour
                .Verify(processor => processor
                    .Execute(_command, It.IsNotNull<Func<Task>>(), _ct));
        }

        [Fact]
        public void UseManyBehaviour()
        {
            var behaviours = Many(5, MockBehaviour);
            var pipeline = new CommandFullPipeline<Command>(
                behaviours.Select(mock => mock.Object).ToArray(),
                new[] {_preProcessor.Object},
                _processor.Object,
                new[] {_postProcessor.Object});

            pipeline
                .Awaiting(pipe => pipe.Execute(_command, _ct))
                .Should().NotThrow();

            foreach (var behaviour in behaviours)
            {
                behaviour
                    .Verify(processor => processor
                        .Execute(_command, It.IsNotNull<Func<Task>>(), _ct));
            }
        }

        [Fact]
        public void UseManyPreProcessor()
        {
            var preProcessors = Many(5, () => MockCommandPreProcessor(_command, _ct));
            
            var pipeline = new CommandFullPipeline<Command>(
                new[] {_behaviour.Object},
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
            
            var pipeline = new CommandFullPipeline<Command>(
                new[] {_behaviour.Object},
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


        private Mock<ICommandBehaviour<Command>> MockBehaviour()
        {
            var behaviour = new Mock<ICommandBehaviour<Command>>();

            behaviour
                .Setup(processor => processor
                    .Execute(_command, It.IsNotNull<Func<Task>>(), _ct))
                .Returns<Command, Func<Task>, CancellationToken>((command, next, ct) => next());

            return behaviour;
        }
    }
}