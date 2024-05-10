using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.AgentApi.Extensions;
using TGS.Core.Models.Exceptions;
using TGS.Services.Agent;

namespace TGS.Apis.AgentApi.Middlewares;

public sealed class MaintenanceMiddleware(RequestDelegate next)
{
    private static readonly ActivitySource _MaintenanceMiddlewareSource =
        new($"TGS.Apis.AgentApi.Middlewares.{nameof(MaintenanceMiddleware)}");

    public async Task InvokeAsync(HttpContext context,
        IMaintenanceRedisRepository maintenanceRedisService)
    {
        using var activity = _MaintenanceMiddlewareSource.StartActivity(nameof(MaintenanceMiddleware));

        var brandId = context.Items.GetBrandId();

        if (!string.IsNullOrEmpty(await maintenanceRedisService.GetBrandMaintenanceAsync(brandId)
                .ConfigureAwait(false))) throw new BrandUnderMaintenanceException("Brand is put into maintenance");
        await next(context);
    }
}