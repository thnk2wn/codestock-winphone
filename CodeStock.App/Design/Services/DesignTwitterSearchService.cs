using System;
using System.Collections.Generic;
using CodeStock.Data;
using CodeStock.Data.Model;
using CodeStock.Data.ServiceAccess;

namespace CodeStock.App.Design.Services
{
    public class DesignTwitterSearchService : ITwitterSearchService
    {

        #region ITwitterSearchService Members

        public void Search(string query)
        {
            this.Result = new TwitterSearchResult
            {
                Tweets = new List<Tweet>
                {
                    new Tweet
                        {
                            Text = "Just booked my codestock trip. My third trip to the Knox convention... #codestock11",
                            ProfileImageUrl = "/Images/unisex-48x48.png",
                            FromUser = "RoboticsGirl",
                            CreatedAt = DateTime.Parse("Tue, 03 May 2012 01:19:06 +0000"),
                            Source = @"&lt;a href=&quot;http://twitter.com/#!/download/iphone&quot; rel=&quot;nofollow&quot;&gt;Twitter for iPhone&lt;/a&gt;"
                        },
                    new Tweet
                        {
                            Text = "@rickborup #SWF 2012 - I've sure got the fever. BTW- Are you going to CodeStock in June?",
                            ProfileImageUrl = "http://a2.twimg.com/profile_images/640539890/Geoff_-_Jan_18_2010__2a__normal.jpg",
                            FromUser = "mattslay",
                            ToUser = "rickborup",
                            CreatedAt = DateTime.Parse("Tue, 03 May 2012 01:19:06 +0000"),
                            Source = @"lt;a href=&quot;http://www.echofon.com/&quot; rel=&quot;nofollow&quot;&gt;Echofon&lt;/a&gt;"
                        }
                }
            
            };

            if (null != AfterCompleted)
                AfterCompleted(new CompletedEventArgs());
        }

        public TwitterSearchResult Result {get; private set;}
        

        public Action<CompletedEventArgs> AfterCompleted {get; set;}

        #endregion
    }
}
