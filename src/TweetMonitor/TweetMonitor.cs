using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitterTriggerExtension;
using Tweetinvi.Events;
using System.Linq;

namespace TweetMonitor
{
    public static class TweetMonitor
    {
        [FunctionName("TweetMonitor")]
        public static async Task Run([TwitterTrigger(filter: "#azurefunctions")]MatchedTweetReceivedEventArgs tweetEvent, ILogger log)
        {
            log.LogInformation($"\n@{tweetEvent.Tweet.CreatedBy.ScreenName}[{tweetEvent.Tweet.CreatedBy.Name}]\n" +
                $"Tweeted: {tweetEvent.Tweet.FullText}\n" +
                $"HashTags: [{String.Join(",", tweetEvent.Tweet.Hashtags.Select(x => $"#{x.Text}"))}]");
        }
    }
}
