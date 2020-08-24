using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using TGS.Infrastructures.Json;

namespace TGS.Infrastructures.Unity
{
    public abstract class BaseWorkersStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            CustomConfigureServices(services);
            AddHealthCheckServices(services.AddHealthChecks());
            services.AddControllers();
        }

        public virtual void CustomConfigureServices(IServiceCollection services)
        {
        }

        public virtual void ConfigureApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public virtual void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
        {
            CustomConfigure(app, appLifetime);
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = async (context, report) =>
                    {
                        var jsonSerializationProvider = app.ApplicationServices.GetService<IJsonSerializationProvider>();
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(jsonSerializationProvider.SerializeToString(report));
                    }
                });
            });
            appLifetime.ApplicationStarted.Register(() => OnApplicationStarted(app.ApplicationServices));
            appLifetime.ApplicationStopped.Register(() => OnApplicationStopped(app.ApplicationServices));
        }

        public virtual void CustomConfigure(IApplicationBuilder app, IHostApplicationLifetime appLifetime)
        {
        }

        protected virtual void AddHealthCheckServices(IHealthChecksBuilder healthChecksBuilder)
        {
        }

        protected virtual void OnApplicationStarted(IServiceProvider serviceProvider)
        {
        }

        protected virtual void OnApplicationStopped(IServiceProvider serviceProvider)
        {
        }

        public static IHostBuilder CreateHostBuilder<T, TFactory>(string[] args)
            where T : BaseWorkersStartup
            where TFactory : BaseFactory
            =>
                Host.CreateDefaultBuilder(args)
                    .ConfigureLogging((hostcontext, loggin) =>
                    {
                        loggin.AddConfiguration(hostcontext.Configuration.GetSection("Logging"));
                        if (hostcontext.HostingEnvironment.IsDevelopment())
                        {
                            loggin.AddConsole();
                            loggin.SetMinimumLevel(LogLevel.Error);
                        }

                        loggin.AddConfiguration();
                    })
                    .UseServiceProviderFactory(new UnityContainerFactory<TFactory>())
                    .ConfigureWebHostDefaults(c => c.UseKestrel(k => k.AddServerHeader = false))
                    .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<T>(); });
    }
}