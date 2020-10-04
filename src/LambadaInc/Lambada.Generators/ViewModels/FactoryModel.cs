using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lambada.Generators.ViewModels
{
    public class FactoryModel: TableEntity
    {
        public string FactoryId
        {
            get => RowKey;
            set => RowKey = value;
        }
    
        public string Name { get; set; }
        public string Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int DeviceCount { get; set; }
        public int ItemsProduced { get; set; }
        public DateTime DateCreated { get; set; }
    }
}