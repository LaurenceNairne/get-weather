using System.Runtime.Serialization;

namespace aspnet_thirdpartyapi.Models
{
    [DataContract]
    public class Currently
    {
        [DataMember(Name = "time")]
        public int Time { get; set; }

        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        [DataMember(Name = "temperature")]
        public string Temperature { get; set; }

        [DataMember(Name = "humidity")]
        public string Humidity { get; set; }
    }
}