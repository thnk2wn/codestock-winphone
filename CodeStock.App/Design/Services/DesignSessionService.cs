using System;
using System.Collections.Generic;
using CodeStock.Data.Model;
using CodeStock.Data.ServiceAccess;

namespace CodeStock.App.Design.Services
{
    public class DesignSessionService 
        : DesignServiceBase<Session>, ISessionsService
    {

        public override void Load()
        {
            var list = new List<Session>();

            // too lazy to create accurate design time speakers for each session and who cares anyway
            // could consider something shared to create speaker design time data since sessions and speakers both do this
            var randomSpeaker = new Speaker
            {
                Name = "James Ashley",
                Company = "Razorfish",
                PhotoUrl = "http://codestock.org/Assets/Speakers/009db91d-fbe7-4b78-9066-c550e1777156.jpg",
                TwitterId = "jamesashley",
                Website = "http://www.imaginativeuniversal.com",
                Bio = "James Ashley is a Presentation Layer Architect for the Razorfish Emerging Experiences Team, based in Atlanta, Georgia. " +
                "James runs the Silverlight Atlanta User Group with Corey Schuman. He is also the lead organizer of ReMIX Atlanta. He specializes " +
                "in WP7, Surface, Kiosk and Tablet programming with WPF and Silverlight. He also dabbles with the Kinect."
            };

            list.Add(new Session
            {
                Track = "Developer",
                Area = "Web - ASP.NET MVC",
                Title = "Build Your Own Application Framework with ASP.NET MVC 3",
                LevelGeneral = "Intermediate",
                LevelSpecific = "Beginner",
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(1).AddHours(1),
                Technology = "asp.net mvc, C#",
                Speaker = randomSpeaker,
                Room = "400A",
                VoteRank = "TOP20",
                Abstract = "<b>You’ve finally done it</b>. You sold it to your fellow developers. You fought with your boss and finally won his buy-in. " +
                "You’re finally making the switch to <b>ASP.NET MVC</b>. You promised your boss more features, less bugs, and faster turn-around. But " +
                "there’s a surprise waiting for you. ASP.NET MVC is much less a “framework” than a set of building blocks that you can use to create an " +
                "actual framework. If you don’t invest in building up a proper framework for your application, you will encounter maintenance woes that " +
                "make even WebForms look tame. In this session, I’ll show you how to take full advantage of what ASP.NET MVC brings to the table while " +
                "creating a robust, extensible application framework. You’ll see how to push common concerns into your framework and away from your day-to-day " +
                "development activities. Topics we’ll cover include establishing conventions for <b>generating your UI</b>, how to leverage new " +
                "<b>dependency-injection features of MVC 3</b>, separating concerns with an <b>application bus</b>, and we’ll even cover some <b>jQuery black magic</b>. "
            });

            list.Add(new Session
            {
                Track = "Developer",
                Area = "Applications - Silverlight",
                Title = "A beginner's guide to Silverlight",
                LevelGeneral = "Beginner",
                LevelSpecific = "Beginner",
                StartTime = DateTime.Now.AddDays(2),
                EndTime = DateTime.Now.AddDays(1).AddHours(1),
                Technology = "Silverlight, C#",
                Speaker =  randomSpeaker,
                Room = "406",
                Abstract = "Want to get started developing apps for the web or Windows Phone 7 and don't know much about Silverlight or XNA? Come to this session " +
                "and we'll bring you up to speed with Silverlight. We'll show layout principals and containers, controls, styling & theming, data binding, and MVVM."
            });

            list.Add(new Session
            {
                Track = "Developer",
                Area = "Applications - Silverlight",
                Title = "Expression Blend and the Visual State Manager: A Deep Dive",
                LevelGeneral = "Intermediate",
                LevelSpecific = "Intermediate",
                StartTime = DateTime.Now.AddDays(2),
                EndTime = DateTime.Now.AddDays(1).AddHours(1),
                Technology = ".Net, WPF, Silverlight, Blend",
                Speaker = randomSpeaker,
                Room = "406",
                Abstract = "The Visual State Manager (VSM) is a must have tool in your Silverlight and WPF arsenal. In this session, we will take a deep dive into " +
                "using the VSM in Expression Blend. Topics will include using the VSM for Templating, creating custom States, using the GoToState Behavior, and applying Transitions and Effects."
            });

            this.Data = list;

            OnAfterCompleted();
        }

        public void EnsureCached()
        {
            return;
        }
        
    }
}
