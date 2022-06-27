using RabbitMQ.Client.Events;

namespace BusinessLibrary
{
    public interface IQueueProcessorService
    {
        void ProcessMessage(object model, BasicDeliverEventArgs ea);
    }
}
