using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Apis.BackendOffice.Middlewares;
using TGS.Core.Infrastructures.Extensions;

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
            services.AddBackendOfficeServices(_configuration, _env);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (_env.IsDevelopmentOrDocker())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "UploadImages")),
                RequestPath = "/UploadImages"
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });

            var requestLocalizationOptions =
                app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(requestLocalizationOptions);
        }
    }
}