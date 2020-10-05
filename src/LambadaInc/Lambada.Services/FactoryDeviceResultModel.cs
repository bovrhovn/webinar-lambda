using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lambada.Services
{
    public class FactoryDeviceResultModel : TableEntity
    {
        public string FactoryDeviceResultId
        {
            get => RowKey;
            set => RowKey = value;
        }
        public DateTime DateCreated { get; set; }
        public int Quantity { get; set; }
        public string FactoryDeviceId { get; set; }
    }
}