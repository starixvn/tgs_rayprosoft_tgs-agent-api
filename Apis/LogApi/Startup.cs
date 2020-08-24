using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TGS.Apis.LogApi.DataService;
using TGS.Apis.LogApi.Filters;
using TGS.Apis.LogApi.Services;
using TGS.Infrastructures.Log;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Nats;
using TGS.Infrastructures.Redis;
using TGS.Services.Operations.Extensions;

namespace TGS.Apis.LogApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LoggerServiceConfig>(Configuration.GetSection("LoggerServiceConfig"));
            services.AddRepository<ILogRepository, LogRepository>(Configuration, "Log");
            services.AddRedis();
            services.AddNats(Configuration);

            services.AddTransient<Action<LogBusEvent>>();
            services.AddScoped<ITgsLog, TgsLog>();

            services.AddControllers(options =>
                    options.Filters.Add(new HttpResponseExceptionFilter()))
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var result = new BadRequestObjectResult(context.ModelState);

                        // TODO: add `using System.Net.Mime;` to resolve MediaTypeNames
                        result.ContentTypes.Add(MediaTypeNames.Application.Json);
                        result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                        return result;
                    };
                });

            services.AddMvcCore(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(new HttpResponseExceptionFilter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
