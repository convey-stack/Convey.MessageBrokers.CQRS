using System;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.MessageBrokers.CQRS.Dispatchers;
using Convey.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Convey.MessageBrokers.CQRS
{
    public static class Extensions
    {
        public static Task SendAsync<TCommand>(this IBusPublisher busPublisher, TCommand command, ICorrelationContext context) 
            where TCommand : class, ICommand
            => busPublisher.PublishAsync(command, context);
        
        public static Task PublishAsync<TEvent>(this IBusPublisher busPublisher, TEvent @event, ICorrelationContext context) 
            where TEvent : class, IEvent
            => busPublisher.PublishAsync(@event, context);

        public static IBusSubscriber SubscribeCommand<TCommand>(this IBusSubscriber busSubscriber, 
            string @namespace = null, string queueName = null, Func<TCommand, ConveyException, object> onError = null) 
            where TCommand : class, ICommand
        {
            busSubscriber.SubscribeMessage<TCommand>(async (sp, command, ctx) =>
            {
                var commandHandler = sp.GetService<ICommandHandler<TCommand>>();
                await commandHandler.HandleAsync(command);
            }, @namespace, queueName, onError);

            return busSubscriber;
        }

        public static IBusSubscriber SubscribeEvent<TEvent>(this IBusSubscriber busSubscriber, 
            string @namespace = null, string queueName = null, Func<TEvent, ConveyException, object> onError = null) 
            where TEvent : class, IEvent
        {
            busSubscriber.SubscribeMessage<TEvent>(async (sp, @event, ctx) =>
            {
                var commandHandler = sp.GetService<IEventHandler<TEvent>>();
                await commandHandler.HandleAsync(@event);
            }, @namespace, queueName, onError);

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