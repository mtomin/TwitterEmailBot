using BusinessLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using BusinessLibrary.Interfaces;
using Tweetinvi;
using Tweetinvi.Models;

namespace QueueHandler
{
    class Program
    {
        private static ServiceProvider _serviceProvider;
        private static ILogger _logger;
        private static IQueueProcessorService _queueProcessorService;
        private static IConfigurationRoot config;

        static void Main(string[] args)
        {
            config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<Program>()
                .Build();

            ConfigureServices();

            var messagingService = new MessagingService(new ConnectionFactory());
            messagingService.ConfigureService("mailExchange", "mailQueue", "mailRoutingKey");

            _logger.LogInformation(" [*] Waiting for mails.");
            messagingService.SubscribeConsumer(_queueProcessorService.ProcessMessage);
            messagingService.StartConsuming();
            _logger.LogInformation(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void ConfigureServices()
        {
            #region Register services
            _serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                        .AddConsole();
                })
                .AddSingleton<TwitterClient>(sp =>
                {
                    var userCredentials = new TwitterCredentials(config.GetValue<string>("appKey"), config.GetValue<string>("appSecret"), 
                        config.GetValue<string>("accessToken"), config.GetValue<string>("accessTokenSecret"));
                    return new TwitterClient(userCredentials);
                })
                .AddSingleton<ISocialNetworkClient, TwitterClientWrapper>()
                .AddSingleton<IQueueProcessorService, QueueProcessorService>()
                .AddScoped<ISocialNetworkPublisherService, SocialNetworkPublisherService>()
                .BuildServiceProvider();
            #endregion

            #region Assign services
            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
            _queueProcessorService = _serviceProvider.GetService<IQueueProcessorService>();
            #endregion
        }
    }
}
