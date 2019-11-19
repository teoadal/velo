using System;
using System.Collections.Concurrent;
using Velo.Utils;

namespace Velo.CQRS.Notifications
{
    internal sealed class NotificationRouter : IDisposable
    {
        public static readonly Type[] ProcessorTypes =
        {
            typeof(INotificationProcessor<>)
        };

        private static readonly Type PublisherType = typeof(NotificationPublisher<>);

        private Func<Type, INotificationPublisher> _buildPublisher;
        private ConcurrentDictionary<Type, INotificationPublisher> _publishers;

        public NotificationRouter()
        {
            _buildPublisher = BuildPublisher;
            _publishers = new ConcurrentDictionary<Type, INotificationPublisher>();
        }

        public NotificationPublisher<TNotification> GetPublisher<TNotification>()
            where TNotification : INotification
        {
            var notificationType = Typeof<TNotification>.Raw;
            var publisher = _publishers.GetOrAdd(notificationType, _buildPublisher);

            return (NotificationPublisher<TNotification>) publisher;
        }

        private static INotificationPublisher BuildPublisher(Type notificationType)
        {
            var publisherType = PublisherType.MakeGenericType(notificationType);
            return (INotificationPublisher) Activator.CreateInstance(publisherType);
        }

        public void Dispose()
        {
            _buildPublisher = null!;

            CollectionUtils.DisposeValuesIfDisposable(_publishers);
            _publishers.Clear();
            _publishers = null!;
        }
    }
}