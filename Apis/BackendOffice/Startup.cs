using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBackendOfficeServices(_configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopmentOrDocker())
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
                endpoints.MapHealthChecks("/health",  new()
                {
                    ResultStatusCodes = Consts.HealthCheckResultStatusCodes,
                    ResponseWriter = (httpContext, result) =>
                        HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, _env)
                });

                endpoints.MapMetrics();
            });

            var requestLocalizationOptions =
                app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(requestLocalizationOptions);
        }
    }
}