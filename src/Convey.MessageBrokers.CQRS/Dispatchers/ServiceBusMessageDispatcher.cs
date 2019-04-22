using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;

namespace Convey.MessageBrokers.CQRS.Dispatchers
{
    internal sealed class ServiceBusMessageDispatcher : ICommandDispatcher, IEventDispatcher
    {
        private readonly IBusPublisher _busPublisher;
        private readonly ICorrelationContextAccessor _accessor;

        public ServiceBusMessageDispatcher(IBusPublisher busPublisher, ICorrelationContextAccessor accessor)
        {
            _busPublisher = busPublisher;
            _accessor = accessor;
        }

        Task ICommandDispatcher.DispatchAsync<T>(T command)
            => _busPublisher.SendAsync(command, _accessor.CorrelationContext ?? CorrelationContext.Empty);

        Task IEventDispatcher.DispatchAsync<T>(T @event)
            => _busPublisher.PublishAsync(@event, _accessor.CorrelationContext ?? CorrelationContext.Empty);
    }
}