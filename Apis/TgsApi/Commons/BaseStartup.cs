using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using NLog;
using TGS.Core.DataServices.Bet.Extensions;
using TGS.Core.DataServices.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Log.Middleware;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.TimeZone;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Services.GameOperations.Extensions;
using TGS.Services.Operations.Extensions;

namespace TGS.Apis.TgsApi.Commons
{
    public abstract class BaseStartup
    {
        protected IWebHostEnvironment Environment { get; }
        protected IConfiguration Configuration { get; }
        protected abstract string LogPrefix { get; }

        protected BaseStartup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Environment = env;
            Configuration = configuration;

            LogManager.Configuration.Variables["connectionString"] = Configuration.GetConnectionString("Log");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Func<string>>(c => () => LogPrefix);

            services.AddTgsRepository(Configuration, "Tgs")
                    .AddBetRepository(Configuration, "Bet");

            services.AddNats(Configuration)
                    .AddWebApiServices()
                    .AddTgsLog()
                    .AddJson()
                    .AddRedis()
                    .AddSecurity()
                    .AddOperations()
                    .AddTimeZone()
                    .AddGameOperations();

            services.AddHttpContextAccessor();
            services.AddScoped<ApiLoggerMiddleware>();

            AddCustomDependencyServices(services);

            services.AddFeatureManagement();

            var gpCode = Configuration.GetValue<string>(Consts.EnvironmentVariable.GameProviderCode);

            services.AddControllers(options =>
            {
                AddCustomFilters(options);
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new BadRequestObjectResult(context.ModelState);
                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    return result;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopmentOrDocker())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseGlobalExceptionHandler();
            }

            app.UseApiLogger();

            UseCustomMiddlewares(app);

            app
                .UseHttpsRedirection()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
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