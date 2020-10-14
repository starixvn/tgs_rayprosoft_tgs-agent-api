using System;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TGS.Apis.LogApi.DataService;
using TGS.Apis.LogApi.Filters;
using TGS.Apis.LogApi.Interfaces;
using TGS.Apis.LogApi.Loggers;
using TGS.Apis.LogApi.Services;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Log.Events;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.LogApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LoggerServiceConfig>(Configuration.GetSection("LoggerServiceConfig"));
            services.AddRepository<ILogRepository, LogRepository>(Configuration, "Log");
            
            services.AddRedis()
                .AddNats(Configuration)
                .AddWebApiServices()
                .AddTgsLog();

            services.AddScoped<IMessagePackProvider, MessagePackProvider>();
            services.AddScoped<IMessageHandler, NatsMessageHandler>();

            services.AddScoped<ApiLogWriter>();
            services.AddScoped<LogRecordWriter>();
            services.AddScoped<AdminLogWriter>();

            services.AddScoped<Func<string, ILoggerWriter>>(serviceProvider => key =>
            {
                return key switch
                {
                    nameof(ApiLogEvent) => serviceProvider.GetService<ApiLogWriter>(),
                    nameof(LogRecordEvent) => serviceProvider.GetService<LogRecordWriter>(),
                    nameof(AdminLogEvent) => serviceProvider.GetService<AdminLogWriter>(),
                    _ => throw new KeyNotFoundException()
                };
            });

            services
                .AddControllers(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                    options.Filters.Add(new HttpResponseExceptionFilter());
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var result = new BadRequestObjectResult(context.ModelState);

                        // TODO: add `using System.Net.Mime;` to resolve MediaTypeNames
                        result.ContentTypes.Add(MediaTypeNames.Application.Json);
                        result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                        return result;
                    };
                });
        }

        protected void OnApplicationStarted(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var messageHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();
            messageHandler.StartReceiveMessages();
        }

        protected void OnApplicationStopped(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var messageHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();
            messageHandler.StopReceiveMessages();
        }

        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env, 
            IHostApplicationLifetime appLifetime,
            ILogRepository repository)
        {
            //repository.ExecuteMigrationAndSeeding();

            appLifetime.ApplicationStarted.Register(() => OnApplicationStarted(app.ApplicationServices));
            appLifetime.ApplicationStopped.Register(() => OnApplicationStopped(app.ApplicationServices));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
