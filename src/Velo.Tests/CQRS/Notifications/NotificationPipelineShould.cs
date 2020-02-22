using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Velo.DependencyInjection;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;
using Xunit.Abstractions;

namespace Velo.CQRS.Notifications
{
    public class NotificationPipelineShould : TestClass
    {
        private readonly DependencyProvider _provider;

        public NotificationPipelineShould(ITestOutputHelper output) : base(output)
        {
            var processor = new Mock<INotificationProcessor<Notification>>();

            _provider = new DependencyCollection()
                .AddEmitter()
                .AddScoped(ctx => processor.Object)
                .BuildProvider();
        }

        [Fact]
        public async Task DisposedAfterCloseScope()
        {
            NotificationPipeline<Notification> pipeline;
            using (var scope = _provider.CreateScope())
            {
                pipeline = scope.GetRequiredService<NotificationPipeline<Notification>>();
            }

            await Assert.ThrowsAsync<NullReferenceException>(
                () => pipeline.Publish(It.IsAny<Notification>(), CancellationToken.None));
        }
    }
}