using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.MessageBrokers.CQRS.Dispatchers;
using Microsoft.Extensions.DependencyInjection;

namespace Convey.MessageBrokers.CQRS
{
    public static class Extensions
    {
        public static Task SendAsync<TCommand>(this IPublisher publisher, TCommand command, object context)
            where TCommand : class, ICommand
            => publisher.PublishAsync(command, context: context);

        public static Task PublishAsync<TEvent>(this IPublisher publisher, TEvent @event, object context)
            where TEvent : class, IEvent
            => publisher.PublishAsync(@event, context: context);

        public static ISubscriber SubscribeCommand<T>(this ISubscriber subscriber) where T : class, ICommand
            => subscriber.Subscribe<T>((sp, command, ctx) => sp.GetService<ICommandHandler<T>>().HandleAsync(command));

        public static ISubscriber SubscribeEvent<T>(this ISubscriber subscriber) where T : class, IEvent
            => subscriber.Subscribe<T>((sp, @event, ctx) => sp.GetService<IEventHandler<T>>().HandleAsync(@event));

        public static IConveyBuilder AddServiceBusCommandDispatcher(this IConveyBuilder builder)
        {
            builder.Services.AddTransient<ICommandDispatcher, ServiceBusMessageDispatcher>();
            return builder;
        }

        public static IConveyBuilder AddServiceBusEventDispatcher(this IConveyBuilder builder)
        {
            builder.Services.AddTransient<IEventDispatcher, ServiceBusMessageDispatcher>();
            return builder;
        }
    }
}