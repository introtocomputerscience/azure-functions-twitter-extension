using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterTriggerExtension
{
    public static class TwitterWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddTwitter(this IWebJobsBuilder builder)
        {
            if(builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<TwitterExtensionConfigProvider>();

            return builder;
        }
    }
}
