using System;
using System.IO;
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
using TGS.Apis.FileService.DataService.Model;
using TGS.Apis.FileService.DataService.Services;
using TGS.Apis.FileService.Extensions;
using TGS.Apis.FileService.Middlewares;
using TGS.Core.Infrastructures.Aws;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Models;
using TGS.FileService.Repositories;

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
                _configuration.GetSection("DatabaseSettings"));

            services.AddSingleton<IDatabaseSettings>(provider =>
                provider.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IDatabaseSettings>();
                return new MongoClient(settings.ConnectionString);
            });

            services.AddTransient(c =>
                c.GetService<IMongoClient>()?.StartSession());

            services
                .AddNats(_configuration)
                .AddTgsLog()
                .AddFile(_configuration)
                .AddAws(_configuration);

            services.AddFileServiceCore()
                .ConfigureAwsOptions((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    config.GetSection("AWS").Bind(options);
                    options.ExternalPublicServiceUrl = config.GetValue<string>("AWS:Project:ExternalServiceUrl");
                    options.ExternalPrivateServiceUrl = config.GetValue<string>("AWS:Project:ExternalServiceUrl");
                    options.PublicBucket = config.GetValue<string>("AWS:Project:Bucket");
                    options.PrivateBucket = config.GetValue<string>("AWS:Secret:Bucket");
                })
                .UseMongoRepository((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.MongoConnectionString = config.GetValue<string>("DatabaseSettings:ConnectionString");
                });

            services.AddTransient<IMongoContext<StationLetterFile>, MongoContext<StationLetterFile>>();
            services.AddTransient<IMongoContext<PromotionFile>, MongoContext<PromotionFile>>();
            services.AddTransient<IUploadService, UploadService>();
            services.AddTransient<IStationLetterService, StationLetterService>();
            services.AddTransient<IPromotionService, PromotionService>();

            services.AddControllers()
                .AddFluentValidation(c => { c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()); });

            services
                .AddHealthChecks()
                .AddMongoDb(_configuration, "DatabaseSettings")
                .AddAwsS3()
                .ForwardToPrometheus();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() {Title = "TGS FileService API", Version = "v1"});

                var xml = "TGS.Apis.FileService.xml";
                var path = Path.Combine(AppContext.BaseDirectory, xml);
                if (File.Exists(path))
                {
                    c.IncludeXmlComments(path);
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseHttpMetrics();

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

            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS FileService API"));
            }
        }
    }
}