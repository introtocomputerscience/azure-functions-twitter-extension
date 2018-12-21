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

            var credentials = new TwitterCredentials(consumerKey, consumerSecret, accessKey, accessSecret);
            _filteredStream = Stream.CreateFilteredStream(credentials);
            _filteredStream.AddTrack(_attribute.Filter);

            if (!string.IsNullOrWhiteSpace(_attribute.User))
            {
                _filteredStream.AddFollow(new UserIdentifier(_attribute.User));
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
                _filteredStream.StopStream();
            };

            await _filteredStream.StartStreamMatchingAllConditionsAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _filteredStream.StopStream();
            return Task.CompletedTask;
        }

    }
}
