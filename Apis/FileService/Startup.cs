using System;
using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Prometheus;
using TGS.Apis.FileService.DataService;
using TGS.Apis.FileService.DataService.Services;
using TGS.Apis.FileService.Extensions;
using TGS.Apis.FileService.Services;
using TGS.Core.Infrastructures.Aws;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Models;

namespace TGS.Apis.FileService
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
            services.Configure<DatabaseSettings>(
                _configuration.GetSection(nameof(DatabaseSettings)));
            services.AddSingleton<IDatabaseSettings>(provider =>
                provider.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            services.AddSingleton<IMongoClient>(c =>
            {
                var settings = _configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
                return new MongoClient(settings.ConnectionString);
            });

            services.AddTransient(c =>
                c.GetService<IMongoClient>()?.StartSession());

            services
                .AddNats(_configuration)
                .AddTgsLog()
                .AddFile(_configuration)
                .AddAws(_configuration);

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IUploadService, UploadService>();
            services.AddTransient<INatsMessageHandler, NatsMessageHandler>();

            services.AddControllers()
                .AddFluentValidation(c => { c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()); });

            services.AddHealthChecks()
                .AddNats()
                .AddMongoDb(_configuration, "DatabaseSettings")
                .AddAwsS3()
                .ForwardToPrometheus(); ;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStarted.Register(() => OnApplicationStarted(app.ApplicationServices));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/fs/health", new()
                {
                    ResultStatusCodes = Consts.HealthCheckResultStatusCodes,
                    ResponseWriter = (httpContext, result) =>
                        HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, env)
                });

                endpoints.MapMetrics();
            });
        }

        protected void OnApplicationStarted(IServiceProvider serviceProvider)
        {
            var messageHandler = serviceProvider.GetRequiredService<INatsMessageHandler>();
            messageHandler.SubscribeAsync();
        }
    }
}