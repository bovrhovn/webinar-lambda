using System;
using Newtonsoft.Json;

namespace Lambada.Generators.ViewModels
{
    public class EventGridModel
    {
        public EventGridModel()
        {
            Id = Guid.NewGuid().ToString();
            EventTime = DateTime.Now;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
 
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "eventtime")]
        public DateTime EventTime { get; set; }
    }
}