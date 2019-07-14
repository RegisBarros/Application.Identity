using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Application.Identity.Api.Models
{
    public static class IdentityRoles
    {
        public const string Admin = nameof(Admin);
        public const string Member = nameof(Member);

        public static IEnumerable<string> GetRoles()
        {
            FieldInfo[] rolesClass = typeof(IdentityRoles).GetFields();

            foreach (var role in rolesClass)
            {
                yield return role.GetValue(typeof(IdentityRoles)).ToString();
            }
        }

        public static string GetRole(string role)
        {
            var roles = GetRoles();

            return roles.First(r => r.Equals(role, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}