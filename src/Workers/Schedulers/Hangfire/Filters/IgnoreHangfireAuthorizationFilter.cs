using Hangfire.Dashboard;

namespace TGS.Workers.Schedulers.Hangfire.Filters;

public sealed class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}
