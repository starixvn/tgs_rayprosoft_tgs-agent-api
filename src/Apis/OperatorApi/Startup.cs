using System;
using System.IO;
using System.Reflection;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;
using Tgs.Payment;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Filters.Swaggers;
using TGS.Apis.OperatorApi.Middlewares;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Services;
using TGS.Core.DataServices.Report.Extensions;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Payment.ManualDeposit;
using TGS.Services.Operations.Extensions;
using TGS.Services.Operator;
using TGS.Services.PluggableServices.Extensions;
using TGS.Services.Report.Application.Extensions;
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

        public void Configure(IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();
            app.UseMiddleware<OperatorApiErrorMapperMiddleware>();

            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.ApplyCurrentCultureToResponseHeaders = true;
            });

            app.UseApiLogger(bypassRouting: Consts.DefaultBypassRouting);

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseCors();

            app.UseVerifyJwtTokenMiddlewares(Consts.DefaultBypassRouting);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new()
                {
                    ResultStatusCodes = GlobalConsts.HealthCheckResultStatusCodes,
                    ResponseWriter = (httpContext, result) =>
                        HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, _env)
                });
                endpoints.MapMetrics();
            });

            if (!_env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS Operator API"));
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTgsRepository(_configuration, "Tgs");
            services.AddReportRepository(_configuration, "Report");

            services.AddScoped<IInvitationCodeValidator, InvitationCodeValidator>();

            services.AddNats(_configuration)
                .AddWebApiServices()
                .AddTgsLog()
                .AddRedis()
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddOperations()
                .AddErrorMappers()
                .AddApiClient()
                .AddPluggableServices()
                .AddReportServices();

            services
                .AddOperatorApiCore()
                .ConfigureOperatorOptions<IConfiguration>((options, config) =>
                {
                    config.GetSection("Operator").Bind(options);
                })
                .ConfigureEmailConfiguration<IConfiguration>((options, config) =>
                {
                    config.GetSection("EmailSetting").Bind(options);
                })
                .UseOperatorRepositories()
                .ConfigurePayment(config =>
                {
                    config.UseTgsPaymentRepository();
                })
                .ConfigureWallet(config =>
                {
                    config.AddTgsWalletRepositories();
                })
                .Services
                .Decorate<IPaymentClient, ManualDepositPaymentClientDecorator>();

            services
                .AddOptions<ConstValidateCodeOptions>()
                .Configure<IConfiguration>((options, config) =>
                {
                    config.GetSection("Operator").Bind(options);
                })
                .Services
                .Decorate<IValidateCodeGenerator, ConstValidateCodeGenerator>();

            services.AddIdentityVerifiers();

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers(options => options.Filters.Add<ValidateModelFilter>())
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                .AddFluentValidation(c =>
                {
                    c.RegisterValidatorsFromAssemblyContaining<Startup>(lifetime: ServiceLifetime.Scoped);
                    c.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    c.LocalizationEnabled = false;
                    c.ImplicitlyValidateChildProperties = true;
                });

            services.AddHttpContextAccessor();

            services.AddMediatR(typeof(Startup).Assembly);

            services.AddHealthChecks()
                .AddPostgres(_configuration, "Tgs", "tgs-db")
                .AddRedis(_configuration)
                .ForwardToPrometheus();

            if (!_env.IsProduction())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new() { Title = "TGS Operator API", Version = "v1" });

                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Name = "Api Authorization",
                        Description = "authorized jwt token, get from api/auth/jwt",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "bearer",
                        Reference = new()
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme
                        }
                    };

                    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

                    c.OperationFilter<SwaggerHeaderFilter>();
                    c.SchemaFilter<SwaggerEnumDescriptionFilter>();
                    c.OperationFilter<SwaggerTgsResponseWrapperFilter>();

                    var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var path = Path.Combine(AppContext.BaseDirectory, xml);
                    if (File.Exists(path))
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

            services.AddSingleton<OperatorApiErrorMapper>();
        }
    }
}
