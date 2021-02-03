using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using TGS.Apis.LogApi.Filters;
using TGS.Apis.LogApi.Interfaces;
using TGS.Apis.LogApi.Loggers;
using TGS.Apis.LogApi.Services;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Log;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Log.Events;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;

namespace TGS.Apis.LogApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LoggerServiceConfig>(_configuration.GetSection("LoggerServiceConfig"));
            services.AddRepository<ILogRepository, LogRepository>(_configuration, "Log");

            services.AddRedis()
                .AddNats(_configuration)
                .AddWebApiServices()
                .AddTgsLog();

            services.AddSingleton<IMessageHandler, NatsMessageHandler>();

            services.AddScoped<ApiLogWriter>();
            services.AddScoped<LogRecordWriter>();
            services.AddScoped<AdminLogWriter>();

            services.AddTransient<Func<string, ILoggerWriter>>(serviceProvider => key =>
            {
                return key switch
                {
                    nameof(ApiLogEvent)    => serviceProvider.GetRequiredService<ApiLogWriter>(),
                    nameof(LogRecordEvent) => serviceProvider.GetRequiredService<LogRecordWriter>(),
                    nameof(AdminLogEvent)  => serviceProvider.GetRequiredService<AdminLogWriter>(),
                    _                      => throw new KeyNotFoundException()
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

                        result.ContentTypes.Add(MediaTypeNames.Application.Json);
                        result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                        return result;
                    };
                });

            services.AddHealthChecks()
                .AddPostgres(_configuration, "Log", name: "log-db")
                .AddRedis(_configuration)
                .AddNats()
                .ForwardToPrometheus(); ;
        }

        private static void OnApplicationStarted(IServiceProvider serviceProvider)
        {
            var messageHandler = serviceProvider.GetRequiredService<IMessageHandler>();
            messageHandler.StartReceiveMessages();
        }

        private static void OnApplicationStopped(IServiceProvider serviceProvider)
        {
            var messageHandler = serviceProvider.GetRequiredService<IMessageHandler>();
            messageHandler.StopReceiveMessages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStarted.Register(() => OnApplicationStarted(app.ApplicationServices));
            appLifetime.ApplicationStopped.Register(() => OnApplicationStopped(app.ApplicationServices));

            app.UseGlobalExceptionHandler();

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health", new()
                {
                    ResultStatusCodes = Consts.HealthCheckResultStatusCodes,
                    ResponseWriter = (httpContext, result) => HealthCheckResponseWriter.CreateResponseAsync(httpContext,
                        result, env, sp =>
                        {
                            var handler = sp.GetRequiredService<IMessageHandler>();
                            IReadOnlyDictionary<string, object> data = new Dictionary<string, object>
                            {
                                {"last_error", handler.LastLogInsertError},
                                {"last_log_insert_time", handler.LastLogInsertTime},
                                {"last_event", handler.LastBusEvent?.GetType().Name ?? string.Empty},
                                {"last_received_event_time", handler.LastReceivedEventTime},
                                {"last_startup_time", handler.LastStartUpTime},
                                {"log_received_count", handler.LogReceivedCount}
                            };

                            return Task.FromResult(data);
                        })
                });

                endpoints.MapMetrics();
            });
        }
    }
}