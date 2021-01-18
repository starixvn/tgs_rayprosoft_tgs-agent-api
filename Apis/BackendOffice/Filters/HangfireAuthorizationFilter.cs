using System.Linq;
using Hangfire.Dashboard;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Domain;

namespace TGS.Apis.BackendOffice.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var claims = httpContext.User.Claims.ToDictionary(p => p.Type, p => p.Value);
            var roleType = claims.GetTypeByKey<RoleType>(Consts.UserInfo.RoleTypeKey);

            return roleType == RoleType.Admin;
        }
    }
}