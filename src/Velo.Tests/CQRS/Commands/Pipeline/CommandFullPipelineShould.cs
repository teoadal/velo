using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CommandFullPipelineShould : CQRSTestClass
    {
        private readonly Command _command;

        private readonly ICommandPipeline<Command> _pipeline;

        private readonly Mock<ICommandBehaviour<Command>> _behaviour;
        private readonly Mock<ICommandPreProcessor<Command>> _preProcessor;
        private readonly Mock<ICommandProcessor<Command>> _processor;
        private readonly Mock<ICommandPostProcessor<Command>> _postProcessor;

        public CommandFullPipelineShould()
        {
            _command = Fixture.Create<Command>();

            _behaviour = MockCommandBehaviour(_command);
            _preProcessor = MockCommandPreProcessor(_command);
            _processor = MockCommandProcessor(_command);
            _postProcessor = MockCommandPostProcessor(_command);

            _pipeline = BuildPipeline(new[] {_behaviour}, new[] {_preProcessor}, _processor, new[] {_postProcessor});
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
        public void DisposeAgainNotAffect()
        {
            for (var i = 0; i < 5; i++)
            {
                _pipeline
                    .Invoking(pipeline => pipeline.Dispose())
                    .Should().NotThrow();
            }
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
        }

        [Fact]
        public void PreProcess()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, CancellationToken))
                .Should().NotThrow();

            _preProcessor.Verify(processor => processor.PreProcess(_command, CancellationToken));
        }

        [Fact]
        public void Process()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, CancellationToken))
                .Should().NotThrow();

            _processor.Verify(processor => processor.Process(_command, CancellationToken));
        }

        [Fact]
        public void PostProcess()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, CancellationToken))
                .Should().NotThrow();

            _postProcessor.Verify(processor => processor.PostProcess(_command, CancellationToken));
        }

        [Fact]
        public void UseBehaviour()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, CancellationToken))
                .Should().NotThrow();

            _behaviour
                .Verify(behaviour => behaviour
                    .Execute(_command, It.IsNotNull<Func<Task>>(), CancellationToken));
        }

        [Fact]
        public void UseManyBehaviours()
        {
            var behaviours = Many(() => MockCommandBehaviour(_command));

            var pipeline = BuildPipeline(behaviours);

            pipeline
                .Awaiting(pipe => pipe.Execute(_command, CancellationToken))
                .Should().NotThrow();

            foreach (var behaviour in behaviours)
            {
                behaviour
                    .Verify(processor => processor
                        .Execute(_command, It.IsNotNull<Func<Task>>(), CancellationToken));
            }
        }

        [Fact]
        public void UseManyPreProcessor()
        {
            var preProcessors = Many(() => MockCommandPreProcessor(_command));

            var pipeline = BuildPipeline(preProcessors: preProcessors);

            pipeline
                .Awaiting(pipe => pipe.Execute(_command, CancellationToken))
                .Should().NotThrow();

            foreach (var preProcessor in preProcessors)
            {
                preProcessor.Verify(processor => processor.PreProcess(_command, CancellationToken));
            }
        }

        [Fact]
        public void UseManyPostProcessor()
        {
            var postProcessors = Many(() => MockCommandPostProcessor(_command));

            var pipeline = BuildPipeline(postProcessors: postProcessors);

            pipeline
                .Awaiting(pipe => pipe.Execute(_command, CancellationToken))
                .Should().NotThrow();

            foreach (var postProcessor in postProcessors)
            {
                postProcessor.Verify(processor => processor.PostProcess(_command, CancellationToken));
            }
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _pipeline.Dispose();

            _pipeline
                .Awaiting(pipeline => pipeline.Execute(_command, CancellationToken))
                .Should().Throw<ObjectDisposedException>();
        }


        private ICommandPipeline<Command> BuildPipeline(
            IEnumerable<Mock<ICommandBehaviour<Command>>> behaviours = null,
            IEnumerable<Mock<ICommandPreProcessor<Command>>> preProcessors = null,
            Mock<ICommandProcessor<Command>> processor = null,
            IEnumerable<Mock<ICommandPostProcessor<Command>>> postProcessors = null)
        {
            return new CommandFullPipeline<Command>(
                behaviours == null
                    ? new[] {_behaviour.Object}
                    : behaviours.Select(b => b.Object).ToArray(),
                preProcessors == null
                    ? new[] {_preProcessor.Object}
                    : preProcessors.Select(p => p.Object).ToArray(),
                processor?.Object ?? _processor.Object,
                postProcessors == null
                    ? new[] {_postProcessor.Object}
                    : postProcessors.Select(p => p.Object).ToArray());
        }

        private void EnsurePipelineExecuted(int count = 1)
        {
            _behaviour
                .Verify(behaviour => behaviour
                    .Execute(_command, It.IsNotNull<Func<Task>>(), CancellationToken), Times.Exactly(count));

            _preProcessor
                .Verify(processor => processor
                    .PreProcess(_command, CancellationToken), Times.Exactly(count));

            _processor
                .Verify(processor => processor
                    .Process(_command, CancellationToken), Times.Exactly(count));

            _postProcessor
                .Verify(processor => processor
                    .PostProcess(_command, CancellationToken), Times.Exactly(count));
        }
    }
}