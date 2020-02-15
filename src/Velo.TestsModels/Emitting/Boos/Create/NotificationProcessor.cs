using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Notifications;
using Velo.Logging;

namespace Velo.TestsModels.Emitting.Boos.Create
{
    public class NotificationProcessor : INotificationProcessor<Notification>
    {
        private readonly ILogger<NotificationProcessor> _logger;

        public NotificationProcessor(ILogger<NotificationProcessor> logger)
        {
            _logger = logger;
        }

        public ValueTask Process(Notification notification, CancellationToken cancellationToken)
        {
            _logger.Debug(nameof(NotificationProcessor));
            return new ValueTask();
        }
    }
}