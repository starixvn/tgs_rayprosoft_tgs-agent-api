using Hangfire.Dashboard;

namespace TGS.Core.Infrastructures.Hangfire.Filters
{
    public sealed class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context) => true;
    }
}
