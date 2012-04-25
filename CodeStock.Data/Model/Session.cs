using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Serialization;

namespace CodeStock.Data.Model
{

    public class Session
    {
        public string Abstract { get; set; }

        //
        //public int AdditionalSpeakerIDs { get; set; }

        
        public string Area { get; set; }
       
        public string LevelGeneral { get; set; }
        
        public string LevelSpecific { get; set; }
        
        public string Room { get; set; }
        
        [JsonProperty("SessionID")]
        public int SessionId { get; set; }
        
        [JsonProperty("SpeakerID")]
        public int SpeakerId { get; set; }

        private DateTime _startTimeUtc;
        private DateTime _startTime;

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                _startTimeUtc = value.ToUniversalTime();
                _startTime = _startTimeUtc.Subtract(EasternTimeUtcOffSet);
            }
        }

        public DateTime StartTimeUtc 
        {
            get { return _startTimeUtc; }

            set
            {
                _startTimeUtc = value;
                _startTime = _startTimeUtc.Subtract(EasternTimeUtcOffSet);
            }
        }

        private DateTime _endTimeUtc;
        private DateTime _endTime;

        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                _endTimeUtc = value.ToUniversalTime();
                _endTime = _endTimeUtc.Subtract(EasternTimeUtcOffSet);
            }
        }

        public DateTime EndTimeUtc
        {
            get { return _endTimeUtc; }

            set
            {
                _endTimeUtc = value;
                _endTime = _endTimeUtc.Subtract(EasternTimeUtcOffSet);
            }
        }


        private static TimeSpan EasternTimeUtcOffSet
        {
            get { return TimeSpan.FromHours(4); }
        }
        
        public string Technology { get; set; }
        
        public string Title { get; set; }
        
        public string Track { get; set; }

        public string Url { get; set; }
        
        /// <summary>
        /// TOP5, TOP20, TOP1, NONE
        /// </summary>
        public string VoteRank { get; set; }

        // no need to serialize speaker; it gets serialized through speakers and reassociated later
        [DoNotSerialize]
        [JsonIgnore]
        public Speaker Speaker { get; set; }

        public override string ToString()
        {
            return string.Format("Session: {0} ({1})", this.Title, this.SessionId);
        }
    }

    [Serializer(typeof(IEnumerable<Session>))]
    public sealed class SessionSerializer : ISerializeObject
    {
        public object[] Serialize(object target)
        {
            var tmp = (IEnumerable<Session>)target;
            return new object[] { tmp.ToList() };
        }

        public object Deserialize(object[] data)
        {
            var tmp = new List<Session>();

            var items = (List<Session>)data[0];
            items.ForEach(tmp.Add);

            return tmp;
        }
    }

    
}
