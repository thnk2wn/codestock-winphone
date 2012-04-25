using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CodeStock.Data.Model
{
    public class Tweet
    {
        [JsonProperty("from_user_id")]
        public int FromUserId { get; set; }

        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("from_user")]
        public string FromUser { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("to_user")]
        public string ToUser { get; set; }

        [JsonProperty("to_user_id")]
        public int? ToUserId { get; set; }
    }

    public class TwitterSearchResult
    {
        [JsonProperty("results")]
        public List<Tweet> Tweets { get; set; }
    }
}
