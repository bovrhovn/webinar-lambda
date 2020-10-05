using Newtonsoft.Json;

namespace Lambada.Models
{
    public class DeviceTagProperty
    {
        [JsonProperty("tags")]
        public Tags Tags { get; set; }
    }

    public class FactoryData
    {
        [JsonProperty("id")] public string FactoryId { get; set; }
        [JsonProperty("model")] public string Model { get; set; }
        [JsonProperty("created")] public string Created { get; set; }
    }

    public class Tags
    {
        [JsonProperty("factory")] public FactoryData FactoryData { get; set; }
    }
}