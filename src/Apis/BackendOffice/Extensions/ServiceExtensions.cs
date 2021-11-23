using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Apis.BackendOffice.DataService.Services;
using TGS.Apis.BackendOffice.Middlewares;
using TGS.Apis.BackendOffice.Services;
using TGS.Clients.Report.Client.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.BackOffice.Extensions;
using TGS.Services.Operations.Extensions;
using TGS.Services.PluggableServices.Extensions;

namespace TGS.Apis.BackendOffice.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBackendOfficeServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTgsRepository(configuration, "Tgs");

            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<ExceptionMiddleware>();

            services.AddTransient(typeof(ITgsCommands<>), typeof(TgsCommands<>));
            services.AddTransient(typeof(ITgsQueries<>), typeof(TgsQueries<>));
            services.AddTransient(typeof(ILogQueries), typeof(LogQueries));
            services.AddTransient<IResellerService, ResellerService>();
            services.AddTransient<ILicenseeService, LicenseeService>();
            services.AddTransient<IBrandPaymentService, BrandPaymentService>();
            services.AddTransient<IStationLetterService, StationLetterService>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IFilterFactory, AutoValidateAntiforgeryTokenAttribute>();

            services
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddWebApiServices()
                .AddTgsLog()
                .AddRedis()
                .AddOperations()
                .AddApiClient()
                .AddPluggableServices()
                .AddTransactions()
                .AddBackOfficeApplicationServices()
                .AddReportClientServices(configuration)
                .AddPromotionClientServices((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    config.GetSection("Promotion").Bind(options);
                })
                .AddPlayerBucketRepositoriesServices()
                .AddCashServiceClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.BaseUrl = config.GetValue<Uri>("BackOffice:CashBaseUrl");
                });

            services.AddWallet()
                .AddTgsAgentWalletRepositories();

            services
                .AddPaymentClient();

            return services;
        }
    }
}
