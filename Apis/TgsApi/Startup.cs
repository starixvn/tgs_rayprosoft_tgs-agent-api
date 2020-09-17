using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.Filters;
using TGS.Core.DataServices.DataInitializers;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Log.Middleware;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.TimeZone;
using TGS.Services.GameOperations.Extensions;
using TGS.Services.Operations.Extensions;

namespace TGS.Apis.TgsApi
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
            
            LogManager.Configuration.Variables["connectionString"] = Configuration.GetConnectionString("Log");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRepositories(Configuration)
                    .AddNats(Configuration)
                    .AddTgsLog()    
                    .AddJson()
                    .AddRedis()
                    .AddOperations()
                    .AddTimeZone()
                    .AddGameOperations();            

            if (_env.IsDevelopment())
            { 
                services.AddScoped<IDataInitializer, DataInitializer>();
            }

            services.AddScoped<ApiLoggerMiddleware>();

            services.AddControllers(options => options.Filters.Add(new HttpResponseExceptionFilter()))
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var result = new BadRequestObjectResult(context.ModelState);
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
        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.SeedData();
            }

            app.UseApiLogger();

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
