# Azure Function Twitter Trigger Extension
This repo contains a twitter based binding extension for <b>Azure WebJobs SDK</b> built on top of [Tweetinvi](https://github.com/linvi/tweetinvi) and using the Twitter Stream API. Addittionally a simple sample <b>Azure Function</b> project demos how to use the trigger.

### Note: I am still actively working on the features of what is here. Please provide feedback and open issues as you find them. I will happilly accept pull requests.

## Configuration
In order to use this trigger you will need to sign up for a [Twitter Developer Account](https://developer.twitter.com) and obtain your Consumer keys and generate Access keys. You then need to update your local.settings.json to contain the following:
```
"TwitterConsumerKey": "<API key>",
"TwitterConsumerSecret": "<API secret key>",
"TwitterAccessKey": "<Access token>",
"TwitterAccessSecret": "<Access token secret>"
```

## TwitterTrigger
```csharp
// Runs when a tweet containing the word "azure" is detected
public static async Task Run([TwitterTrigger("azure")]MatchedTweetReceivedEventArgs tweetEvent, ILogger log)
{
    log.LogInformation($"\n@{tweetEvent.Tweet.CreatedBy.ScreenName}[{tweetEvent.Tweet.CreatedBy.Name}]\n" +
        $"Tweeted: {tweetEvent.Tweet.FullText}\n" +
        $"HashTags: [{String.Join(",", tweetEvent.Tweet.Hashtags.Select(x => $"#{x.Text}"))}]");
}

// Runs when a tweet containing the hashtag "#azure" is detected
public static async Task Run([TwitterTrigger("#azure")]MatchedTweetReceivedEventArgs tweetEvent, ILogger log)
{
    log.LogInformation($"\n@{tweetEvent.Tweet.CreatedBy.ScreenName}[{tweetEvent.Tweet.CreatedBy.Name}]\n" +
        $"Tweeted: {tweetEvent.Tweet.FullText}\n" +
        $"HashTags: [{String.Join(",", tweetEvent.Tweet.Hashtags.Select(x => $"#{x.Text}"))}]");
}

// Runs when a tweet by @introtocs is detected
public static async Task Run([TwitterTrigger(user: "introtocs")]MatchedTweetReceivedEventArgs tweetEvent, ILogger log)
{
    log.LogInformation($"\n@{tweetEvent.Tweet.CreatedBy.ScreenName}[{tweetEvent.Tweet.CreatedBy.Name}]\n" +
        $"Tweeted: {tweetEvent.Tweet.FullText}\n" +
        $"HashTags: [{String.Join(",", tweetEvent.Tweet.Hashtags.Select(x => $"#{x.Text}"))}]");
}

// Runs when a tweet by @introtocs is detected containing the hashtag "#azure"
public static async Task Run([TwitterTrigger("#azure", "introtocs")]MatchedTweetReceivedEventArgs tweetEvent, ILogger log)
{
    log.LogInformation($"\n@{tweetEvent.Tweet.CreatedBy.ScreenName}[{tweetEvent.Tweet.CreatedBy.Name}]\n" +
        $"Tweeted: {tweetEvent.Tweet.FullText}\n" +
        $"HashTags: [{String.Join(",", tweetEvent.Tweet.Hashtags.Select(x => $"#{x.Text}"))}]");
}
```

# License
This project is licensed under the [MIT License](LICENSE.txt)
