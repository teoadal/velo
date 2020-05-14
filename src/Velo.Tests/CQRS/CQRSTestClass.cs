using System.Threading;
using System.Threading.Tasks;
using Moq;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.CQRS.Queries;
using Velo.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS
{
    // ReSharper disable once InconsistentNaming
    public abstract class CQRSTestClass : TestClass
    {
        protected CQRSTestClass(ITestOutputHelper output) : base(output)
        {
        }

        protected Mock<ICommandProcessor<TCommand>> MockCommandProcessor<TCommand>(TCommand command,
            CancellationToken ct)
            where TCommand : notnull, ICommand
        {
            var processor = new Mock<ICommandProcessor<TCommand>>();
            processor.Setup(p => p.Process(command, ct))
                .Returns(Task.CompletedTask);

            return processor;
        }

        protected Mock<ICommandPreProcessor<TCommand>> MockCommandPreProcessor<TCommand>(TCommand command,
            CancellationToken ct)
            where TCommand : notnull, ICommand
        {
            var preProcessor = new Mock<ICommandPreProcessor<TCommand>>();

            preProcessor
                .Setup(processor => processor.PreProcess(command, ct))
                .Returns(Task.CompletedTask);

            return preProcessor;
        }

        protected Mock<ICommandPostProcessor<TCommand>> MockCommandPostProcessor<TCommand>(TCommand command,
            CancellationToken ct)
            where TCommand : notnull, ICommand
        {
            var postProcessor = new Mock<ICommandPostProcessor<TCommand>>();
            postProcessor
                .Setup(processor => processor.PostProcess(command, ct))
                .Returns(Task.CompletedTask);

            return postProcessor;
        }

        protected Mock<INotificationProcessor<TNotification>> MockNotificationProcessor<TNotification>(
            TNotification notification, CancellationToken ct)
            where TNotification : notnull, INotification
        {
            var processor = new Mock<INotificationProcessor<TNotification>>();
            processor
                .Setup(p => p.Process(notification, ct))
                .Returns(Task.CompletedTask);

            return processor;
        }

        protected Mock<IQueryProcessor<TQuery, TResult>> MockQueryProcessor<TQuery, TResult>(TQuery query,
            TResult result, CancellationToken ct)
            where TQuery : notnull, IQuery<TResult>
        {
            var processor = new Mock<IQueryProcessor<TQuery, TResult>>();
            processor
                .Setup(p => p.Process(query, ct))
                .Returns(Task.FromResult(result));

            return processor;
        }

        protected Mock<IQueryPreProcessor<TQuery, TResult>> MockQueryPreProcessor<TQuery, TResult>(TQuery query,
            CancellationToken ct)
            where TQuery : notnull, IQuery<TResult>
        {
            var preProcessor = new Mock<IQueryPreProcessor<TQuery, TResult>>();

            preProcessor
                .Setup(processor => processor.PreProcess(query, ct))
                .Returns(Task.CompletedTask);

            return preProcessor;
        }

        protected Mock<IQueryPostProcessor<TQuery, TResult>> MockQueryPostProcessor<TQuery, TResult>(TQuery query,
            TResult result, CancellationToken ct)
            where TQuery : notnull, IQuery<TResult>
        {
            var postProcessor = new Mock<IQueryPostProcessor<TQuery, TResult>>();
            postProcessor
                .Setup(processor => processor.PostProcess(query, result, ct))
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