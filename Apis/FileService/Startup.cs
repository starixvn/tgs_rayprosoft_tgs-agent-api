using System;
using System.IO;
using System.Reflection;
using FileService.DataService;
using FileService.DataService.Services;
using FileService.Extensions;
using FileService.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;

namespace FileService
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
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

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IUploadService, UploadService>();

            services
                .AddNats(_configuration)
                .AddTgsLog()
                .AddFile(_configuration)
                .AddAwsService(_configuration, _environment);

            services.AddTransient<INatsMessageHandler, NatsMessageHandler>();

            services.AddControllers()
                .AddFluentValidation(c =>
                {
                    c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStarted.Register(() => OnApplicationStarted(app.ApplicationServices));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "UploadImages")),
                RequestPath = "/UploadImages"
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        protected void OnApplicationStarted(IServiceProvider serviceProvider)
        {
            var messageHandler = serviceProvider.GetRequiredService<INatsMessageHandler>();
            messageHandler.SubscribeAsync();
        }
    }
}