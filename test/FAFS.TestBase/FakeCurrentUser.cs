using System;
using System.Collections.Generic;
using System.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace FAFS
{
    [Dependency(ReplaceServices = true)]
    public class FakeCurrentUser : ICurrentUser, ISingletonDependency
    {
        public bool IsAuthenticated => Id.HasValue;
        public Guid? Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberVerified { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public Guid? TenantId { get; set; }
        public string[] Roles { get; set; }

        public Claim FindClaim(string claimType) => null;
        public Claim[] FindClaims(string claimType) => Array.Empty<Claim>();
        public Claim[] GetAllClaims() => Array.Empty<Claim>();
        public bool IsInRole(string roleName) => false;
    }
}
