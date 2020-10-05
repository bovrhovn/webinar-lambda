using System;
using Lambada.Models;

namespace Lambada.Services
{
    public static class ModelConverter
    {

        public static Factory ToFactory(this FactoryModel eventmodel) =>
            new Factory
            {
                Name = eventmodel.Name,
                Description = eventmodel.Description,
                FactoryId = eventmodel.FactoryId,
                Latitude = eventmodel.Latitude,
                Longitude = eventmodel.Longitude,
                DeviceCount = eventmodel.DeviceCount,
                ItemsProduced = eventmodel.ItemsProduced,
                DateCreated = eventmodel.DateCreated
            };

        public static FactoryModel ToFactoryModel(this Factory eventmodel) =>
            new FactoryModel
            {
                Name = eventmodel.Name,
                Description = eventmodel.Description,
                FactoryId = Guid.NewGuid().ToString(),
                Latitude = eventmodel.Latitude,
                Longitude = eventmodel.Longitude,
                DeviceCount = eventmodel.DeviceCount,
                ItemsProduced = eventmodel.ItemsProduced,
                DateCreated = eventmodel.DateCreated
            };
    }
}