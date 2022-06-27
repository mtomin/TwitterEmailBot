using BusinessLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MailChecker
{
    public class MailCheckWorker : BackgroundService
    {
        private readonly ILogger<MailCheckWorker> _logger;
        private readonly IMessagingService _messagingService;
        private readonly IConfiguration _configuration;
        private readonly IMailProcessingService _mailService;

        public MailCheckWorker(ILogger<MailCheckWorker> logger, IConfiguration configuration, IMessagingService messagingService, IMailProcessingService mailService)
        {
            _logger = logger;
            _configuration = configuration;
            _mailService = mailService;
            _messagingService = messagingService;
            _messagingService.ConfigureService(_configuration["MessagingService:ExchangeName"], _configuration["MessagingService:QueueName"], _configuration["MessagingService:RoutingKey"]);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _mailService.ConnectAsync(_configuration["MailSettings:ImapServer"], 993, true, stoppingToken).ConfigureAwait(false);

                var uids = await _mailService.GetUnreadUidsAsync(_configuration["MailSettings:Username"], _configuration["MailSettings:Password"], stoppingToken).ConfigureAwait(false);

                _logger.LogInformation(uids.Count == 0
                    ? $"No new mails at {DateTime.Now}"
                    : $"Found {uids.Count} unread messages at {DateTime.Now}. Processing...");

                foreach (var uid in uids)
                {
                    await _mailService.ProcessMail(uid, "[MTTweetBot]").ConfigureAwait(false);
                }

                await _mailService.DisconnectAsync(stoppingToken).ConfigureAwait(false);

                _logger.LogInformation($"Finished processing - {DateTime.Now}");

                await Task.Delay(20 * 1000, stoppingToken).ConfigureAwait(false);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _messagingService.CloseConnection();
            await _mailService.DisconnectAsync(cancellationToken);
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
