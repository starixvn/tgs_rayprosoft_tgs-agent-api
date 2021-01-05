using System;
using System.IO;
using System.Reflection;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Filters.Swaggers;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Services;
using TGS.Core.DataServices.Bet.Extensions;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.Clients.Extensions;
using TGS.Services.Operations.Extensions;
using GlobalConsts = TGS.Core.Models.Consts;

namespace TGS.Apis.OperatorApi
{
    public sealed class Startup
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
            services.AddTgsRepository(_configuration, "Tgs")
                .AddBetRepository(_configuration, "Bet");

            services.AddScoped<IInvitationCodeValidator, InvitationCodeValidator>();

            services.AddNats(_configuration)
                .AddWebApiServices()
                .AddTgsLog()
                .AddRedis()
                .AddJson()
                .AddSecurity()
                .AddOperations()
                .AddErrorMappers()
                .AddApiClient()
                .AddPluggalbeGameProviderApiClient();

            services.AddControllers(options => options.Filters.Add<ValidateModelFilter>())
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                .AddFluentValidation(c =>
                {
                    c.RegisterValidatorsFromAssemblyContaining<Startup>(lifetime: ServiceLifetime.Singleton);
                    c.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    c.LocalizationEnabled = false;
                    c.ImplicitlyValidateChildProperties = true;
                });

            services.AddHttpContextAccessor();

            services.AddMediatR(typeof(Startup).Assembly);

            services.AddHealthChecks()
                .AddSqlServer(_configuration, "Tgs", name: "tgs-db")
                .AddSqlServer(_configuration, "Bet", name: "bet-db")
                .AddRedis(_configuration);

            if (!_env.IsProduction())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo {Title = "TGS Operator API", Version = "v1"});

                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Name = "Api Authorization",
                        Description = "authorized jwt token, get from api/auth/jwt",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "bearer",
                        Reference = new OpenApiReference
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme
                        }
                    };

                    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

                    c.OperationFilter<SwaggerHeaderFilter>();
                    c.SchemaFilter<SwaggerEnumDescriptionFilter>();

                    var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var path = Path.Combine(AppContext.BaseDirectory, xml);
                    c.IncludeXmlComments(path);
                });
            }

            services.AddCors(options =>
            {
                var corsDomain = _configuration.GetSection("CorsDomain").Get<CorsDomain>();
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(corsDomain.Urls)
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .SetIsOriginAllowed(_ => true);
                    });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();

            app.UseRouting();

            app.UseCors();

            app.UseVerifyJwtTokenMiddlewares();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResultStatusCodes = GlobalConsts.HealthCheckResultStatusCodes,
                    ResponseWriter = (httpContext, result) =>
                        HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, _env)
                });
            });

            if (!_env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS Operator API"));
            }
        }
    }
}