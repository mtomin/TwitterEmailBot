using MailKit.Net.Imap;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace BusinessLibrary
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddMessagingService(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionFactory, ConnectionFactory>();
            services.AddSingleton<IImapClient>(new ImapClient() { CheckCertificateRevocation = false });
            return services;
        }
    }
}
