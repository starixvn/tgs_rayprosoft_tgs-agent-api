using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TGS.Services.BackOffice.Application.Middlewares;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.BackOffice.Infrastructure.Services;

namespace TGS.Services.BackOffice.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBackOfficeApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Singleton);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PipelineBehavior<,>));
            services.AddTransient<IBetLimitService, BetLimitService>();
            services.AddTransient<IBrandService, BrandService>();
            services.AddTransient<IBrandGameService, BrandGameService>();
            services.AddTransient<IBrandGameProviderService, BrandGameProviderService>();
            services.AddTransient<IBrandGameProviderScheduleService, BrandGameProviderScheduleService>();
            services.AddTransient<IGameProviderService, GameProviderService>();
            services.AddTransient<IGameProviderBetLimitService, GameProviderBetLimitService>();
            services.AddTransient<IUploadService, UploadService>();

            return services;
        }
    }
}