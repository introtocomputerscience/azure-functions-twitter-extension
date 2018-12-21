using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using TwitterTriggerExtension;

[assembly: WebJobsStartup(typeof(TwitterWebJobsStartup), "Twitter")]

namespace TwitterTriggerExtension
{
    public class TwitterWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddTwitter();
        }
    }
}
