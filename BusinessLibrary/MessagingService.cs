using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BusinessLibrary
{
    public class MessagingService : IMessagingService
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private MessagingConfiguration _configuration;
        private EventingBasicConsumer _basicConsumer;

        public MessagingService(IConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void ConfigureService(string exchangeName, string queueName, string routingKey)
        {
            _configuration = new MessagingConfiguration()
            {
                ExchangeName = exchangeName,
                QueueName = queueName,
                RoutingKey = routingKey
            };
            ConfigureService(_configuration);
        }

        public void ConfigureService(MessagingConfiguration config)
        {
            _channel.ExchangeDeclare(config.ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(config.QueueName, config.QueueDurable, config.QueueExclusive, config.QueueAutoDelete, null);
            _channel.QueueBind(config.QueueName, config.ExchangeName, config.RoutingKey, null);
        }

        public void CloseConnection()
        {
            if (_channel.IsOpen)
                _channel.Close();

            if (_connection.IsOpen)
                _connection.Close();
        }

        public void PushMessage(string message)
        {
            var messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(_configuration.ExchangeName, _configuration.RoutingKey, null, messageBodyBytes);
        }

        public void SubscribeConsumer(EventHandler<BasicDeliverEventArgs> delegateMethod)
        {
            _basicConsumer ??= new EventingBasicConsumer(_channel);
            _basicConsumer.Received += delegateMethod;
        }

        public void StartConsuming()
        {
            _channel.BasicConsume(queue: _configuration.QueueName,
                autoAck: true,
                consumer: _basicConsumer);
        }
    }
}
