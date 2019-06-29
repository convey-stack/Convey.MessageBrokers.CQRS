using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.MessageBrokers.CQRS.Dispatchers;
using Microsoft.Extensions.DependencyInjection;

namespace Convey.MessageBrokers.CQRS
{
    public static class Extensions
    {
        public static Task SendAsync<TCommand>(this IBusPublisher busPublisher, TCommand command,
            ICorrelationContext context)
            where TCommand : class, ICommand
            => busPublisher.PublishAsync(command, context);

        public static Task PublishAsync<TEvent>(this IBusPublisher busPublisher, TEvent @event,
            ICorrelationContext context)
            where TEvent : class, IEvent
            => busPublisher.PublishAsync(@event, context);

        public static IBusSubscriber SubscribeCommand<T>(this IBusSubscriber busSubscriber) where T : class, ICommand
        {
            busSubscriber.Subscribe<T>(async (sp, command, ctx) =>
            {
                var commandHandler = sp.GetService<ICommandHandler<T>>();
                await commandHandler.HandleAsync(command);
            });

            return busSubscriber;
        }

        public static IBusSubscriber SubscribeEvent<T>(this IBusSubscriber busSubscriber) where T : class, IEvent
        {
            busSubscriber.Subscribe<T>(async (sp, @event, ctx) =>
            {
                var commandHandler = sp.GetService<IEventHandler<T>>();
                await commandHandler.HandleAsync(@event);
            });

            return busSubscriber;
        }

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