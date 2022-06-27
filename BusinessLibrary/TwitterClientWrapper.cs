using BusinessLibrary.Interfaces;
using System.Threading.Tasks;
using Tweetinvi;

namespace BusinessLibrary
{
    public class TwitterClientWrapper : ISocialNetworkClient
    {
        private readonly TwitterClient _client;
        public TwitterClientWrapper(TwitterClient client)
        {
            _client = client;
        }
        public async Task Post(string content)
        {
            await _client.Tweets.PublishTweetAsync(content);
        }
    }
}
