using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operator;

namespace TGS.Apis.OperatorApi.Middlewares
{
    public sealed class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;

        public MaintenanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,
           IMaintenanceRedisRepository maintenanceRedisService)
        {
            var brandId = context.Items.GetBrandId();

            if (!string.IsNullOrEmpty(await maintenanceRedisService.GetBrandMaintenanceAsync(brandId).ConfigureAwait(false)))
            {
                throw new BrandUnderMaintenanceException();
            }
            await _next(context);
        }
    }
}
