using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.CQRS.Queries;
using Velo.DependencyInjection;
using Xunit;

namespace Velo.Tests.CQRS
{
    // ReSharper disable once InconsistentNaming
    public abstract class CQRSTestClass : TestClass
    {
        protected static Mock<ICommandBehaviour<TCommand>> MockCommandBehaviour<TCommand>(TCommand command)
            where TCommand : ICommand
        {
            var behaviour = new Mock<ICommandBehaviour<TCommand>>();

            behaviour
                .Setup(processor => processor
                    .Execute(command, It.IsNotNull<Func<Task>>(), CancellationToken))
                .Returns<TCommand, Func<Task>, CancellationToken>((cmd, next, ct) => next());

            return behaviour;
        }

        protected static Mock<ICommandProcessor<TCommand>> MockCommandProcessor<TCommand>(TCommand command)
            where TCommand : notnull, ICommand
        {
            var processor = new Mock<ICommandProcessor<TCommand>>();
            processor.Setup(p => p.Process(command, CancellationToken))
                .Returns(Task.CompletedTask);

            return processor;
        }

        protected static Mock<ICommandPreProcessor<TCommand>> MockCommandPreProcessor<TCommand>(TCommand command)
            where TCommand : notnull, ICommand
        {
            var preProcessor = new Mock<ICommandPreProcessor<TCommand>>();

            preProcessor
                .Setup(processor => processor.PreProcess(command, CancellationToken))
                .Returns(Task.CompletedTask);

            return preProcessor;
        }

        protected static Mock<ICommandPostProcessor<TCommand>> MockCommandPostProcessor<TCommand>(TCommand command)
            where TCommand : notnull, ICommand
        {
            var postProcessor = new Mock<ICommandPostProcessor<TCommand>>();
            postProcessor
                .Setup(processor => processor.PostProcess(command, CancellationToken))
                .Returns(Task.CompletedTask);

            return postProcessor;
        }

        protected static Mock<INotificationProcessor<TNotification>> MockNotificationProcessor<TNotification>(
            TNotification notification)
            where TNotification : notnull, INotification
        {
            var processor = new Mock<INotificationProcessor<TNotification>>();
            processor
                .Setup(p => p.Process(notification, CancellationToken))
                .Returns(Task.CompletedTask);

            return processor;
        }

        protected static Mock<IQueryBehaviour<TQuery, TResult>> MockQueryBehaviour<TQuery, TResult>(
            TQuery query,
            TResult result)
            where TQuery : notnull, IQuery<TResult>
        {
            var behaviour = new Mock<IQueryBehaviour<TQuery, TResult>>();

            behaviour
                .Setup(processor => processor
                    .Execute(query, It.IsNotNull<Func<Task<TResult>>>(), CancellationToken))
                .Returns<TQuery, Func<Task<TResult>>, CancellationToken>((q, next, ct) => next());

            return behaviour;
        }

        protected static Mock<IQueryProcessor<TQuery, TResult>> MockQueryProcessor<TQuery, TResult>(
            TQuery query,
            TResult result)
            where TQuery : notnull, IQuery<TResult>
        {
            var processor = new Mock<IQueryProcessor<TQuery, TResult>>();
            processor
                .Setup(p => p.Process(query, CancellationToken))
                .Returns(Task.FromResult(result));

            return processor;
        }

        protected static Mock<IQueryPreProcessor<TQuery, TResult>> MockQueryPreProcessor<TQuery, TResult>(TQuery query)
            where TQuery : notnull, IQuery<TResult>
        {
            var preProcessor = new Mock<IQueryPreProcessor<TQuery, TResult>>();

            preProcessor
                .Setup(processor => processor.PreProcess(query, CancellationToken))
                .Returns(Task.CompletedTask);

            return preProcessor;
        }

        protected static Mock<IQueryPostProcessor<TQuery, TResult>> MockQueryPostProcessor<TQuery, TResult>(
            TQuery query,
            TResult result)
            where TQuery : notnull, IQuery<TResult>
        {
            var postProcessor = new Mock<IQueryPostProcessor<TQuery, TResult>>();
            postProcessor
                .Setup(processor => processor.PostProcess(query, result, CancellationToken))
                .Returns(Task.CompletedTask);

            return postProcessor;
        }

        public static TheoryData<DependencyLifetime> Lifetimes => new TheoryData<DependencyLifetime>
        {
            DependencyLifetime.Scoped,
            DependencyLifetime.Singleton,
            DependencyLifetime.Transient
        };
    }
}