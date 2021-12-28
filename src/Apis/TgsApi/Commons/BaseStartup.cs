using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OpenTelemetry.Exporter.ProfileViewer.Filters;
using OpenTelemetry.Trace;
using Prometheus;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.TimeZone;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Services.Operations.Extensions;

namespace TGS.Apis.TgsApi.Commons
{
    public abstract class BaseStartup
    {
        protected IConfiguration Configuration { get; }

        protected IWebHostEnvironment Environment { get; }

        protected abstract string LogPrefix { get; }

        protected BaseStartup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Environment = env;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();

            app.UseGameApiLogger(
                LogPrefix,
                new[] { "/health", "/favicon.ico", "/metrics" });

            UseCustomMiddlewaresBeforeRouting(app);

            app.UseRouting();

            app.UseHttpMetrics();

            UseCustomMiddlewaresAfterRouting(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health",
                    new HealthCheckOptions
                    {
                        ResultStatusCodes = Consts.HealthCheckResultStatusCodes,
                        ResponseWriter = (httpContext, result) =>
                            HealthCheckResponseWriter.CreateResponseAsync(httpContext,
                                result,
                                Environment,
                                async sp =>
                                {
                                    var gpCode =
                                        Configuration.GetValue<string>(Consts.EnvironmentVariable.GameProviderCode);

                                    var feature = sp.GetRequiredService<IFeatureManager>();

                                    var isFeatureEnabled = gpCode.IsNotNullOrEmpty() &&
                                                           await feature.IsEnabledAsync(gpCode);

                                    return new Dictionary<string, object>
                                    {
                                        { "GameProviderCode", gpCode }, { "FeatureEnabled", isFeatureEnabled }
                                    };
                                })
                    });

                endpoints.MapMetrics();
                endpoints.MapProfileViewer();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTgsRepository(Configuration, "Tgs");

            services
                .AddWebApiServices()
                .AddTgsLog()
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddTransactions()
                .AddOperations()
                .AddTimeZone()
                .AddApiClient()
                .AddGameApiClients()
                .AddGameOperations()
                .AddGameProvidersoRepository()
                .AddCashServiceClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.BaseUrl = config.GetValue<Uri>("Game:CashBaseUrl");
                });

            services
                .AddRedis((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.ConnectionString = config.GetConnectionString("RedisConfigurationString");
                });

            services.AddHttpContextAccessor();

            AddCustomDependencyServices(services);

            services.AddFeatureManagement();

            services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelFilter>();
                AddCustomFilters(options);
            }).AddXmlSerializerFormatters().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
            });

            services.AddHealthChecks()
                .AddPostgres(Configuration, "Tgs", "tgs-db")
                .AddRedis(Configuration)
                .ForwardToPrometheus();

            services
                .AddMessageQueue()
                .AddNatsMessageQueue(natsConfig =>
                {
                    natsConfig
                        .ConfigQueueOptions((options, sp) =>
                        {
                            var config = sp.GetRequiredService<IConfiguration>();

                            options.Url = config.GetValue<string>("NatsSetting:Urls");
                        });
                });

            services
                .AddOpenTelemetryTracing(builder
                    => builder
                        .AddSource("TGS.*")
                        .AddSource("Tgs.*")
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation(options =>
                        {
                            options.SetDbStatementForText = true;
                        })
                        .AddProfileViewExporter(profileViewBuilder =>
                        {
                            profileViewBuilder
                                .AddFilter(new UrlPathFilter("/health"));
                        }))
                .AddProfileViewer();
        }

        #region Abstract and Virtual Methods

        protected virtual void AddCustomDependencyServices(IServiceCollection services)
        {
        }

        protected virtual void AddCustomFilters(MvcOptions options)
        {
        }

        protected virtual void UseCustomMiddlewaresAfterRouting(IApplicationBuilder app)
        {
        }

        protected virtual void UseCustomMiddlewaresBeforeRouting(IApplicationBuilder app)
        {
        }

        #endregion Abstract and Virtual Methods
    }
}
