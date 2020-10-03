using System.Security.Claims;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.AspNetCore.Http;

namespace Lambada.Base
{
    public class UserDataContext : IUserDataContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserDataContext(IHttpContextAccessor httpContextAccessor) => 
            this.httpContextAccessor = httpContextAccessor;

        public LambadaUser GetCurrentUser()
        {
            var httpContextUser = httpContextAccessor.HttpContext.User;
            if (httpContextUser == null) return null;

            var currentUser = new LambadaUser();

            var claimName = httpContextUser.FindFirst(ClaimTypes.Name);
            currentUser.FullName = claimName.Value;

            var claimId = httpContextUser.FindFirst(ClaimTypes.NameIdentifier);
            currentUser.UserId = claimId.Value;

            return currentUser;
        }
    }
}