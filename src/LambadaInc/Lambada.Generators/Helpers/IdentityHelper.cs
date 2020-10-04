using System.Collections.Generic;
using System.Security.Claims;
using Lambada.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Lambada.Generators.Helpers
{
    public static class IdentityHelper
    {
        public static ClaimsPrincipal GenerateClaims(this LambadaUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };
            return new ClaimsPrincipal(new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme));
        }
    }
}