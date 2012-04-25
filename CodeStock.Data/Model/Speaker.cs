using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Serialization;

namespace CodeStock.Data.Model
{
    public class Speaker
    {
        public string Bio { get; set; }
        public string Company { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }

        [JsonProperty("SpeakerID")]
        public int SpeakerId { get; set; }

        [JsonProperty("TwitterID")]
        public string TwitterId { get; set; }

        public string Url { get; set; }

        public string Website { get; set; }

        // no longer needed; will be delay loaded elsewhere:
        //[JsonIgnore]
        //public IEnumerable<Session> Sessions { get; set; }

        public override string ToString()
        {
            return string.Format("Speaker: {0} ({1})", this.Name, this.SpeakerId);
        }
    }

    [Serializer(typeof(IEnumerable<Speaker>))]
    public sealed class SpeakerSerializer : ISerializeObject
    {
        public object[] Serialize(object target)
        {
            var tmp = (IEnumerable<Speaker>)target;
            return new object[] { tmp.ToList() };
        }

        public object Deserialize(object[] data)
        {
            var tmp = new List<Speaker>();

            var items = (List<Speaker>)data[0];
            items.ForEach(tmp.Add);

            return tmp;
        }
    }
}
