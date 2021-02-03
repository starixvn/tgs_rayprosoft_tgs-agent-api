using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages.HealthCheck;

namespace TGS.Core.Infrastructures.HealthChecks
{
    public static class HealthCheckResponseWriter
    {
        public static async Task CreateResponseAsync(HttpContext httpContext, HealthReport result,
            IWebHostEnvironment env,
            Func<IServiceProvider, Task<IReadOnlyDictionary<string, object>>> getAdditionalValues = null)
        {
            var additionalValues = getAdditionalValues != null
                ? await getAdditionalValues(httpContext.RequestServices)
                : new Dictionary<string, object>();

            httpContext.Response.ContentType = "application/json; charset=utf-8";

            var hostName = Dns.GetHostName();
            var hostEntry = await Dns.GetHostEntryAsync(hostName);
            var hostIp = Array.Find(hostEntry.AddressList, x => x.AddressFamily == AddressFamily.InterNetwork);

            var response = new HealthCheckResponse
            {
                Data = new()
                {
                    MachineName = Environment.MachineName,
                    HostName = hostName,
                    HostIp = hostIp?.ToString() ?? string.Empty,
                    Environment = env.EnvironmentName,
                    OsVersion = $"{Environment.OSVersion}",
                    Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    ProcessCount = Environment.ProcessorCount,
                    AdditionalValues = additionalValues
                },
                Result = new()
                {
                    Status = result.Status.ToString(),
                    TotalDuration = result.TotalDuration.TotalSeconds,
                    DependencyServices = result.Entries.Select(srv => new DependencyService
                    {
                        Key = srv.Key,
                        Duration = srv.Value.Duration.TotalSeconds,
                        Description = srv.Value.Description,
                        Data = srv.Value.Data,
                        Exception = srv.Value.Exception?.Message,
                        Status = srv.Value.Status.ToString(),
                        Tags = srv.Value.Tags.Any() ? srv.Value.Tags.JoinAsStringByComma() : string.Empty
                    }).ToArray()
                }
            };

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response,
                new() {WriteIndented = true}));
        }
    }
}