using System.Collections.Generic;
using CodeStock.Data.Model;
using CodeStock.Data.ServiceAccess;

namespace CodeStock.App.Design.Services
{
    public class DesignSpeakersService
        : DesignServiceBase<Speaker>, ISpeakersService
    {
        public override void Load()
        {
            var list = new List<Speaker>();

            var sessionsSvc = new DesignSessionService();
            sessionsSvc.Load();

            list.Add(
                new Speaker
                {
                    Bio = "An internationally known author and lecturer, Mike Amundsen travels throughout the United States " +
                    "and Europe consulting and speaking on a wide range of topics including distributed network architecture, " +
                    "Web application development, Cloud computing, and other subjects. His recent work focuses on the role " +
                    "hypermedia plays in creating and maintaining applications that can successsfully evolve over time. He has " +
                    "more than a dozen books to his credit and recently contributed to the book \"RESTful Web Services Cookbook\" " +
                    "(by Subbu Allamaraju). When he is not working, Mike enjoys spending time with his family in Kentucky, USA.",
                    Name = "Mike Amundsen",
                    PhotoUrl = "http://codestock.org/Assets/Speakers/182fb9b0-e6ce-4ee5-94e9-16fb1840972a.jpg",
                    SpeakerId = 19,
                    TwitterId = "@mamund",
                    Website = "http://amundsen.com/blog/"
                    //Sessions = sessionsSvc.Data.Take(1)
                });

            list.Add(
                new Speaker
                {
                    Bio = "James Ashley is a Presentation Layer Architect for the Razorfish Emerging Experiences Team, based in " +
                    "Atlanta, Georgia. James runs the Silverlight Atlanta User Group with Corey Schuman. He is also the lead organizer " +
                    "of ReMIX Atlanta. He specializes in WP7, Surface, Kiosk and Tablet programming with WPF and Silverlight. He also dabbles with the Kinect.",
                    Company = "Razorfish",
                    Name = "James Ashley",
                    PhotoUrl = "http://codestock.org/Assets/Speakers/009db91d-fbe7-4b78-9066-c550e1777156.jpg",
                    SpeakerId = 2,
                    TwitterId = "@jamesashley",
                    Website = "http://www.imaginativeuniversal.com"
                    //Sessions = sessionsSvc.Data.Take(2)
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
