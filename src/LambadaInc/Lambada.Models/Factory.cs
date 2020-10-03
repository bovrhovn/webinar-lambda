using System;

namespace Lambada.Models
{
    public class Factory
    {
        public string FactoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int DeviceCount { get; set; }
        public int ItemsProduced { get; set; }
        public DateTime DateCreated { get; set; }
    }
}