using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TGS.Apis.GlobalApi.Models.Messages.HealthCheck;
using TGS.Core.DataServices.Bet.Extensions;
using TGS.Core.DataServices.Extensions;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.Operations.Extensions;

namespace TGS.Apis.GlobalApi
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTgsRepository(_configuration, "Tgs")
                .AddBetRepository(_configuration, "Bet");

            services.AddJson()
                .AddNats(_configuration)
                .AddWebApiServices()
                .AddTgsLog()
                .AddRedis()
                .AddSecurity()
                .AddErrorMappers()
                .AddOperations();

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddControllers(options => options.Filters.Add<ValidateModelFilter>())
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                .AddFluentValidation(c =>
                {
                    c.RegisterValidatorsFromAssemblyContaining<Startup>();
                    c.LocalizationEnabled = false;
                    c.ImplicitlyValidateChildProperties = true;
                });

            services.AddHealthChecks()
                .AddSqlServer(_configuration,"Tgs", name: "tgs-db")
                .AddSqlServer(_configuration,"Bet", name: "bet-db")
                .AddRedis(_configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseGlobalExceptionHandler();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new()
                {
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError
                    },
                    ResponseWriter = CreateHealthCheckResponse(env)
                });
            });
        }

        private static Func<HttpContext, HealthReport, Task> CreateHealthCheckResponse(IWebHostEnvironment env)
        {
            return (httpContext, result) =>
            {
                httpContext.Response.ContentType = "application/json";

                var hostName = Dns.GetHostName();
                var hostIp = Array.Find(Dns.GetHostEntry(hostName).AddressList,
                    x => x.AddressFamily == AddressFamily.InterNetwork);

                var response = new HealthCheckResponse
                {
                    Data = new()
                    {
                        MachineName = Environment.MachineName,
                        HostName = hostName,
                        HostIp = hostIp?.ToString() ?? string.Empty,
                        Environment = env.EnvironmentName,
                        OsVersion = $"{Environment.OSVersion}",
                        ProcessCount = Environment.ProcessorCount
                    },
                    Result = new()
                    {
                        Status = result.Status.ToString(),
                        TotalDuration = result.TotalDuration.TotalSeconds,
                        DependencyServices = result.Entries.Select(srv => new DependencyService
                        {
                            Key = srv.Key,
                            Duration = srv.Value.Duration.TotalSeconds,
                            Exception = srv.Value.Exception?.Message,
                            Status = srv.Value.Status.ToString(),
                            Tags = srv.Value.Tags.Any() ? srv.Value.Tags.JoinAsStringByComma() : string.Empty
                        }).ToArray()
                    }
                };

                return httpContext.Response.WriteAsync(JsonSerializer.Serialize(response,
                    new() {WriteIndented = true}));
            };
        }
    }
}