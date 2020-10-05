using System;
using Lambada.Generators.ViewModels;
using Lambada.Models;

namespace Lambada.Generators.Helpers
{
    public static class ModelConverter
    {
        public static LambadaUser ToUser(this LambadaUserModel eventmodel) =>
            new LambadaUser
            {
                FullName = eventmodel.FullName,
                UserId = eventmodel.UserId,
                Password = eventmodel.Password,
                Email = eventmodel.Email
            };

        public static LambadaUserModel ToUserModel(this LambadaUser eventmodel) =>
            new LambadaUserModel
            {
                FullName = eventmodel.FullName,
                UserId = Guid.NewGuid().ToString(),
                Password = eventmodel.Password,
                Email = eventmodel.Email
            };

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