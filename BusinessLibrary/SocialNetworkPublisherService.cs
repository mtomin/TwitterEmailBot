using System;
using System.Threading.Tasks;
using BusinessLibrary.Interfaces;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Core.Models;
using Tweetinvi.Models;

namespace BusinessLibrary
{
    public class SocialNetworkPublisherService : ISocialNetworkPublisherService
    {
        private readonly ISocialNetworkClient _client;
        private readonly ILogger _logger;

        public SocialNetworkPublisherService(ISocialNetworkClient client, ILogger<SocialNetworkPublisherService> logger)
        {
            _client = client;
            _logger = logger;
        }
        public async Task Publish(string content)
        {
            try
            {
                await _client.Post(content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error publishing!");
                throw;
            }
        }
    }
}
