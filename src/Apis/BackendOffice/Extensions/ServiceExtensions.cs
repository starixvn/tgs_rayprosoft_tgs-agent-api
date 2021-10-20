using System;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Apis.BackendOffice.DataService.Services;
using TGS.Apis.BackendOffice.Filters;
using TGS.Apis.BackendOffice.Middlewares;
using TGS.Apis.BackendOffice.Services;
using TGS.Clients.Report.Client.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
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
                .AddNats(configuration)
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
                .AddPlayerBucketRepositoriesServices();

            services.AddWallet()
                .AddTgsWalletRepositories()
                .AddTgsAgentWalletRepositories();

            services
                .AddPaymentClient();
                
            return services;
        }
    }
}