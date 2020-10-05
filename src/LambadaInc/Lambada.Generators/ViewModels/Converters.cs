using System;
using Lambada.Models;

namespace Lambada.Generators.ViewModels
{
    public static class Converters
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
    }
}