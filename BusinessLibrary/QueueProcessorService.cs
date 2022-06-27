using RabbitMQ.Client.Events;
using System;
using System.Text;
using BusinessLibrary.Interfaces;
using Microsoft.Extensions.Logging;

namespace BusinessLibrary
{
    public class QueueProcessorService : IQueueProcessorService
    {
        private readonly ILogger _logger;
        private readonly ISocialNetworkPublisherService _publisherService;
        public QueueProcessorService(ILogger<QueueProcessorService> logger, ISocialNetworkPublisherService publisherService)
        {
            _logger = logger;
            _publisherService = publisherService;
        }
        public void ProcessMessage(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _publisherService.Publish(message);
            _logger.LogInformation($"{DateTime.Now} Handled message {message}");
        }
    }
}
