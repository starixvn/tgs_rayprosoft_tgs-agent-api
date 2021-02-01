using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddReportServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient<IMarketService, MarketService>();
            services.AddTransient<IRoundService, RoundService>();

            return services;
        }
    }
}