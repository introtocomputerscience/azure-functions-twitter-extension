using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Logging;
using Tweetinvi.Events;

namespace TwitterTriggerExtension
{
    [Extension("Twitter")]
    internal class TwitterExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly ILoggerFactory _loggerFactory;

        public TwitterExtensionConfigProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var rule = context.AddBindingRule<TwitterTriggerAttribute>();
            rule.BindToTrigger<MatchedTweetReceivedEventArgs>(new TwitterTriggerAttributeBindingProvider(_loggerFactory));

            rule.AddConverter<string, MatchedTweetReceivedEventArgs>(str => TwitterTriggerBinding.GetEventArgsFromString(str));
        }
    }
}
