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
    public class CommandPipelineShould : TestClass
    {
        private readonly Command _command;
        private readonly CancellationToken _ct;

        private readonly Mock<ICommandBehaviour<Command>> _behaviour;
        private readonly Mock<ICommandPreProcessor<Command>> _preProcessor;
        private readonly Mock<ICommandProcessor<Command>> _processor;
        private readonly Mock<ICommandPostProcessor<Command>> _postProcessor;

        private readonly CommandPipeline<Command> _fullPipeline;

        public CommandPipelineShould(ITestOutputHelper output) : base(output)
        {
            _command = new Command();
            _ct = CancellationToken.None;

            _behaviour = BuildBehaviour();
            _preProcessor = BuildPreProcessor();
            _postProcessor = BuildPostProcessor();

            _processor = new Mock<ICommandProcessor<Command>>();
            _processor
                .Setup(processor => processor.Process(_command, _ct))
                .Returns(Task.CompletedTask);

            _fullPipeline = new CommandPipeline<Command>(
                new[] {_behaviour.Object},
                new[] {_preProcessor.Object},
                _processor.Object,
                new[] {_postProcessor.Object});
        }

        [Fact]
        public void ExecuteBehaviour()
        {
            _fullPipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().NotThrow();

            _behaviour.Verify(behaviour => behaviour.Execute(_command, It.IsNotNull<Func<Task>>(), _ct));
        }

        [Fact]
        public void ExecuteWithoutBehaviour()
        {
            var pipeline = new CommandPipeline<Command>(
                Array.Empty<ICommandBehaviour<Command>>(),
                new[] {_preProcessor.Object},
                _processor.Object,
                new[] {_postProcessor.Object});

            pipeline
                .Awaiting(p => p.Execute(_command, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void ExecuteWithProcessorAndPostProcessor()
        {
            var pipeline = new CommandPipeline<Command>(
                Array.Empty<ICommandBehaviour<Command>>(),
                Array.Empty<ICommandPreProcessor<Command>>(),
                _processor.Object,
                new[] {_postProcessor.Object});

            pipeline
                .Awaiting(p => p.Execute(_command, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void ExecuteWithProcessorAndPreProcessor()
        {
            var pipeline = new CommandPipeline<Command>(
                Array.Empty<ICommandBehaviour<Command>>(),
                new[] {_preProcessor.Object},
                _processor.Object,
                Array.Empty<ICommandPostProcessor<Command>>());

            pipeline
                .Awaiting(p => p.Execute(_command, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void ExecuteWithProcessorOnly()
        {
            new CommandPipeline<Command>(_processor.Object)
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void ExecuteWithManyPreProcessors()
        {
            var preProcessors = Enumerable
                .Range(0, 5)
                .Select(_ => BuildPreProcessor())
                .ToArray();

            var pipeline = new CommandPipeline<Command>(
                new[] {_behaviour.Object},
                preProcessors.Select(p => p.Object).ToArray(),
                _processor.Object,
                new[] {_postProcessor.Object});

            pipeline
                .Awaiting(p => p.Execute(_command, _ct))
                .Should().NotThrow();

            foreach (var preProcessor in preProcessors)
            {
                preProcessor.Verify(processor => processor.PreProcess(_command, _ct));
            }
        }

        [Fact]
        public void ExecuteWithManyPostProcessors()
        {
            var postProcessors = Enumerable
                .Range(0, 5)
                .Select(_ => BuildPostProcessor())
                .ToArray();

            var pipeline = new CommandPipeline<Command>(
                new[] {_behaviour.Object},
                new[] {_preProcessor.Object},
                _processor.Object,
                postProcessors.Select(p => p.Object).ToArray());

            pipeline
                .Awaiting(p => p.Execute(_command, _ct))
                .Should().NotThrow();

            foreach (var postProcessor in postProcessors)
            {
                postProcessor.Verify(processor => processor.PostProcess(_command, _ct));
            }
        }

        [Fact]
        public void Send()
        {
            var pipeline = (ICommandPipeline) _fullPipeline;

            pipeline
                .Awaiting(p => p.Send(_command, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void PreProcess()
        {
            _fullPipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().NotThrow();

            _preProcessor.Verify(processor => processor.PreProcess(_command, _ct));
        }

        [Fact]
        public void Process()
        {
            _fullPipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().NotThrow();

            _processor.Verify(processor => processor.Process(_command, _ct));
        }

        [Fact]
        public void PostProcess()
        {
            _fullPipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().NotThrow();

            _postProcessor.Verify(processor => processor.PostProcess(_command, _ct));
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _fullPipeline.Dispose();

            _fullPipeline
                .Awaiting(pipeline => pipeline.Execute(_command, _ct))
                .Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void ThrowIfDisposedWithoutBehaviour()
        {
            var pipeline = new CommandPipeline<Command>(_processor.Object);

            pipeline.Dispose();
            
            pipeline
                .Awaiting(p => p.Execute(_command, _ct))
                .Should().Throw<NullReferenceException>();
        }
        
        private Mock<ICommandBehaviour<Command>> BuildBehaviour()
        {
            var behaviour = new Mock<ICommandBehaviour<Command>>();
            behaviour
                .Setup(b => b.Execute(_command, It.IsNotNull<Func<Task>>(), _ct))
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