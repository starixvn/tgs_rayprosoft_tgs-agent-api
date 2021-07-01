using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Prometheus;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Apis.BackendOffice.Middlewares;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Models;

namespace TGS.Apis.BackendOffice
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopmentOrDocker())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new()
                {
                    ResultStatusCodes = Consts.HealthCheckResultStatusCodes,
                    ResponseWriter = (httpContext, result) =>
                        HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, env)
                });

                endpoints.MapMetrics();
            });

            var requestLocalizationOptions =
                app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(requestLocalizationOptions);

            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS BackOffice API"));
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddBackendOfficeServices(_configuration)
                .AddBackendOfficeRepositories();

            services.AddPaymentServiceClient((options, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                options.PaymentServiceBaseUrl = config.GetValue<Uri>("BackOffice:PaymentServiceBaseUrl");
                options.JsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = {
                        new JsonStringEnumConverter()
                    }
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "TGS BackOffice API", Version = "v1" });

                c.CustomSchemaIds(type => type.FullName.Replace('.', '_'));

                var xml = "TGS.Apis.BackendOffice.xml";
                var path = Path.Combine(AppContext.BaseDirectory, xml);
                if (File.Exists(path))
                {
                    c.IncludeXmlComments(path);
                }
            });
        }
    }
}
