using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Events;

namespace TwitterTriggerExtension
{
    internal class TwitterTriggerBinding : ITriggerBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly ILogger _logger;
        private readonly TwitterTriggerAttribute _attribute;
        private readonly IReadOnlyDictionary<string, Type> _bindingContract;

        public TwitterTriggerBinding(ParameterInfo parameter, ILogger logger)
        {
            _parameter = parameter;
            _logger = logger;
            _attribute = parameter.GetCustomAttribute<TwitterTriggerAttribute>(inherit: false);
            _bindingContract = CreateBindingContract();
        }
        public IReadOnlyDictionary<string, Type> BindingDataContract => _bindingContract;
        public Type TriggerValueType => typeof(MatchedTweetReceivedEventArgs);

        private IReadOnlyDictionary<string, Type> CreateBindingContract()
        {
            Dictionary<string, Type> contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            contract.Add("TwitterTrigger", typeof(MatchedTweetReceivedEventArgs));
            return contract;
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            MatchedTweetReceivedEventArgs tweetEvent = value as MatchedTweetReceivedEventArgs;
            if (tweetEvent == null)
            {
                string tweetInfo = value as string;
                tweetEvent = GetEventArgsFromString(tweetInfo);
            }

            IReadOnlyDictionary<string, object> bindingData = GetBindingData(tweetEvent);

            return Task.FromResult<ITriggerData>(new TriggerData(null, bindingData));
        }

        private IReadOnlyDictionary<string, object> GetBindingData(MatchedTweetReceivedEventArgs value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Dictionary<string, object> bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            bindingData.Add("TwitterTrigger", value);

            return bindingData;
        }

        internal static MatchedTweetReceivedEventArgs GetEventArgsFromString(string tweetInfo)
        {
            if (!string.IsNullOrEmpty(tweetInfo))
            {
                //TODO: Figure this out: https://github.com/Azure/azure-webjobs-sdk-extensions/blob/master/src/WebJobs.Extensions/Extensions/Files/Bindings/FileTriggerBinding.cs
                var w = "a";
            }

            return null;
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            return Task.FromResult<IListener>(new TwitterListener(context.Executor, _attribute));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            string filter = _attribute.Filter;
            string user = _attribute.User;

            return new TwitterTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "Enter a string to filter on",
                    Description = $"Tweet event occured on filter text {filter}",
                    DefaultValue = "#azure"
                }
            };
        }

        private class TwitterTriggerParameterDescriptor : TriggerParameterDescriptor
        {
            public override string GetTriggerReason(IDictionary<string, string> arguments)
            {
                if (arguments != null && arguments.TryGetValue(Name, out var filter))
                {
                    return $"Tweet detected at {DateTime.Now.ToString("o")}";
                }
                return null;
            }
        }
    }
}