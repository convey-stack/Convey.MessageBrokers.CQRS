using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;

namespace Convey.MessageBrokers.CQRS.Dispatchers
{
    internal sealed class ServiceBusMessageDispatcher : ICommandDispatcher, IEventDispatcher
    {
        private readonly IPublisher _publisher;
        private readonly ICorrelationContextAccessor _accessor;

        public ServiceBusMessageDispatcher(IPublisher publisher, ICorrelationContextAccessor accessor)
        {
            _publisher = publisher;
            _accessor = accessor;
        }

        public Task SendAsync<T>(T command) where T : class, ICommand
            => _publisher.SendAsync(command, _accessor.CorrelationContext);

        public Task PublishAsync<T>(T @event) where T : class, IEvent
            => _publisher.PublishAsync(@event, _accessor.CorrelationContext);
    }
}