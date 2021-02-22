using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Prometheus;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.TimeZone;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Services.GameOperations.Extensions;
using TGS.Services.Operations.Extensions;
using TGS.Shared.Game.ApiClients.Extensions;

namespace TGS.Apis.TgsApi.Commons
{
    public abstract class BaseStartup
    {
        protected BaseStartup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Environment = env;
            Configuration = configuration;
        }

        protected IWebHostEnvironment Environment { get; }
        protected IConfiguration Configuration { get; }

        protected abstract string LogPrefix { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTgsRepository(Configuration, "Tgs");

            services.AddNats(Configuration)
                .AddWebApiServices()
                .AddTgsLog()
                .AddJson()
                .AddRedis()
                .AddSecurity()
                .AddTransactions()
                .AddOperations()
                .AddTimeZone()
                .AddApiClient()
                .AddGameApiClients()
                .AddGameOperations();

            services.AddHttpContextAccessor();

            AddCustomDependencyServices(services);

            services.AddFeatureManagement();

            services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelFilter>();
                AddCustomFilters(options);
            }).ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = false; });

            services.AddHealthChecks()
                .AddPostgres(Configuration, "Tgs", "tgs-db")
                .AddRedis(Configuration)
                .AddNats()
                .ForwardToPrometheus();

            services.AddMediatR(typeof(BaseStartup).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();

            app.UseApiLogger(LogPrefix, new[] {"/health", "/favicon.ico"});

            UseCustomMiddlewares(app);

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new()
                {
                    ResultStatusCodes = Consts.HealthCheckResultStatusCodes,
                    ResponseWriter = (httpContext, result) =>
                        HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, Environment, async sp =>
                        {
                            var gpCode = Configuration.GetValue<string>(Consts.EnvironmentVariable.GameProviderCode);

                            var feature = sp.GetRequiredService<IFeatureManager>();
                            var isFeatureEnabled = !gpCode.IsNullOrEmpty() && await feature.IsEnabledAsync(gpCode);

                            return new Dictionary<string, object>
                            {
                                {"GameProviderCode", gpCode},
                                {"FeatureEnabled", isFeatureEnabled}
                            };
                        })
                });

                endpoints.MapMetrics();
            });
        }

        #region Abstract and Virtual Methods

        protected virtual void AddCustomDependencyServices(IServiceCollection services)
        {
        }

        protected virtual void UseCustomMiddlewares(IApplicationBuilder app)
        {
        }

        protected virtual void AddCustomFilters(MvcOptions options)
        {
        }

        #endregion Abstract and Virtual Methods
    }
}