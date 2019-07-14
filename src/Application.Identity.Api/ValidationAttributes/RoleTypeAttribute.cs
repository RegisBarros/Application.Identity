using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Application.Identity.Api.Models;

namespace Application.Identity.Api.ValidationAttributes
{
    public class RoleTypeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if(value == null) return false;

            var role = (string)value;

            if(string.IsNullOrEmpty(role)) return false;

            var roles = IdentityRoles.GetRoles();

            return roles.Any(r => r.Equals(role, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}