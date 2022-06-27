using System;
using RabbitMQ.Client.Events;

namespace BusinessLibrary
{
    public interface IMessagingService
    {
        void ConfigureService(string exchangeName, string queueName, string routingKey);

        void ConfigureService(MessagingConfiguration config);

        void CloseConnection();

        void PushMessage(string message);

        void SubscribeConsumer(EventHandler<BasicDeliverEventArgs> delegateMethod);

        void StartConsuming();
    }
}
