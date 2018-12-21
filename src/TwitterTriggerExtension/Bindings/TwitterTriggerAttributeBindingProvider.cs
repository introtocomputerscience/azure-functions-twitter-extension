using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Tweetinvi.Events;

namespace TwitterTriggerExtension
{
    public class TwitterTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly ILogger _logger;

        public TwitterTriggerAttributeBindingProvider(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger(LogCategories.CreateTriggerCategory("Twitter"));
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException("context");
            }

            //Retrieve Parameter
            ParameterInfo parameter = context.Parameter;
            TwitterTriggerAttribute attribute = parameter.GetCustomAttribute<TwitterTriggerAttribute>(inherit: false);
            if(attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            //Validate Trigger
            if (!IsSupportedBindingType(parameter.ParameterType))
            {
                throw new InvalidOperationException($"Can't bind TwitterTriggerAttribute to type '{parameter.ParameterType}'");
            }

            return Task.FromResult<ITriggerBinding>(new TwitterTriggerBinding(parameter, _logger));
        }

        private bool IsSupportedBindingType(Type t)
        {
            return t == typeof(MatchedTweetReceivedEventArgs);
        }
    }
}
