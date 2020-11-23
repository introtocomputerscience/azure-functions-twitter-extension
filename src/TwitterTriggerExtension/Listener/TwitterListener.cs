using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace TwitterTriggerExtension
{
    public class TwitterListener : IListener
    {
        public ITriggeredFunctionExecutor Executor { get; }

        private TwitterTriggerAttribute _attribute;
        private IFilteredStream _filteredStream;

        public TwitterListener(ITriggeredFunctionExecutor executor, TwitterTriggerAttribute attribute)
        {
            Executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        }

        public void Cancel() { }

        public void Dispose() { }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var consumerKey = Environment.GetEnvironmentVariable("TwitterConsumerKey");
            var consumerSecret = Environment.GetEnvironmentVariable("TwitterConsumerSecret");
            var accessKey = Environment.GetEnvironmentVariable("TwitterAccessKey");
            var accessSecret = Environment.GetEnvironmentVariable("TwitterAccessSecret");

            var userClient = new TwitterClient(consumerKey, consumerSecret, accessKey, accessSecret);

            _filteredStream = userClient.Streams.CreateFilteredStream();

            if (!string.IsNullOrEmpty(_attribute.Filter))
            {
                _filteredStream.AddTrack(_attribute.Filter);
            }

            if (!string.IsNullOrWhiteSpace(_attribute.User))
            {
                var user = await userClient.Users.GetUserAsync(_attribute.User);
                _filteredStream.AddFollow(user);
            }

            _filteredStream.MatchingTweetReceived += async (obj, tweetEvent) =>
            {
                var triggerData = new TriggeredFunctionData
                {
                    TriggerValue = tweetEvent
                };
                await Executor.TryExecuteAsync(triggerData, CancellationToken.None);
            };

            _filteredStream.DisconnectMessageReceived += (obj, disconnectEvent) =>
            {
                _filteredStream.Stop();
            };

            await _filteredStream.StartMatchingAllConditionsAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _filteredStream.Stop();
            return Task.CompletedTask;
        }

    }
}
