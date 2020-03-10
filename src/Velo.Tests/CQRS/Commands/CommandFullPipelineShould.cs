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

            _behaviour = BuildBehaviour();

            _preProcessor = BuildPreProcessor();

            _processor = new Mock<ICommandProcessor<Command>>();
            _processor
                .Setup(processor => processor.Process(_command, _ct))
                .Returns(Task.CompletedTask);

            _postProcessor = BuildPostProcessor();

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
            var behaviours = BuildMany(5, BuildBehaviour);
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
            var preProcessors = BuildMany(5, BuildPreProcessor);
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
            var postProcessors = BuildMany(5, BuildPostProcessor);
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


        private Mock<ICommandBehaviour<Command>> BuildBehaviour()
        {
            var behaviour = new Mock<ICommandBehaviour<Command>>();

            behaviour
                .Setup(processor => processor
                    .Execute(_command, It.IsNotNull<Func<Task>>(), _ct))
                .Returns<Command, Func<Task>, CancellationToken>((command, next, ct) => next());

            return behaviour;
        }

        private Mock<ICommandPreProcessor<Command>> BuildPreProcessor()
        {
            var preProcessor = new Mock<ICommandPreProcessor<Command>>();

            preProcessor
                .Setup(processor => processor.PreProcess(_command, _ct))
                .Returns(Task.CompletedTask);

            return preProcessor;
        }

        private Mock<ICommandPostProcessor<Command>> BuildPostProcessor()
        {
            var postProcessor = new Mock<ICommandPostProcessor<Command>>();
            postProcessor
                .Setup(processor => processor.PostProcess(_command, _ct))
                .Returns(Task.CompletedTask);

            return postProcessor;
        }
    }
}