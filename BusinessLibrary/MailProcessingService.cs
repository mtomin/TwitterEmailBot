using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLibrary
{
    public class MailProcessingService : IMailProcessingService
    {
        private readonly IImapClient _client;
        private readonly IMessagingService _messagingService;
        private readonly ILogger _logger;

        public MailProcessingService(IImapClient client, IMessagingService messagingService, ILogger<MailProcessingService> logger)
        {
            _client = client;
            _messagingService = messagingService;
            _logger = logger;
        }

        public async Task ConnectAsync(string imapServer, int port, bool useSsl, CancellationToken cancellationToken)
        {
            await _client.ConnectAsync(imapServer, 993, useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<string>> GetUnreadUidsAsync(string username, string password, CancellationToken cancellationToken)
        {
            await _client.AuthenticateAsync(username, password, cancellationToken);
            await _client.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken).ConfigureAwait(false);
            var uids = await _client.Inbox.SearchAsync(SearchQuery.NotSeen, cancellationToken).ConfigureAwait(false);
            return uids.Select(u => u.ToString()).ToList();
        }

        public async Task ProcessMail(string uid, string subjectFilter)
        {
            var mailUid = UniqueId.Parse(uid);
            var message = await _client.Inbox.GetMessageAsync(mailUid).ConfigureAwait(false);
            var mailValid = Validate(message, subjectFilter);
            if (mailValid)
            {
                string body = message.TextBody ??
                     HttpUtility.HtmlDecode(
                        Regex.Replace(message.HtmlBody, "<(.|\n)*?>", "")
                    ).Trim();
                _messagingService.PushMessage(body);
                _logger.LogInformation($"Processed message: {message.Subject}");
            }
            await _client.Inbox.AddFlagsAsync(mailUid, MessageFlags.Seen, true).ConfigureAwait(false);
        }

        private bool Validate(MimeMessage message, string subjectFilter)
        {
            if (!message.Subject.Contains(subjectFilter, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Irrelevant subject");
                return false;
            }

            return !(message.TextBody?.Length > 280) && message.TextBody?.Length != 0;
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            if (_client.IsConnected)
                await _client.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);
        }
    }
}
