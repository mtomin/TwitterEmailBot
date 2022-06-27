namespace BusinessLibrary
{
    public class MessagingConfiguration
    {
        public string QueueName { get; init; }

        public string ExchangeName { get; init; }

        public string RoutingKey { get; init; }

        public bool QueueDurable { get; init; }

        public bool QueueExclusive { get; init; }

        public bool QueueAutoDelete { get; set; }
    }
}
