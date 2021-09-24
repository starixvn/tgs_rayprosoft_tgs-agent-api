using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Filters;
using TGS.Apis.AgentApi.Filters.Swaggers;
using TGS.Apis.AgentApi.Models;
using TGS.Clients.Report.Client.Extensions;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.ErrorMappers;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Mail;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.Operations.Extensions;
using TGS.Services.PluggableServices.Extensions;
using GlobalConsts = TGS.Core.Models.Consts;

namespace TGS.Apis.AgentApi
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

        public void Configure(IApplicationBuilder app)
        {
            app.UseTgsErrorHandler();

            app.UseRequestLocalization(options =>
            {
                var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

                options.SupportedCultures.Clear();
                options.SupportedUICultures.Clear();

                config.GetSection("RequestLocalization").Bind(options);
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS Agent API"));
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTgsRepository(_configuration, "Tgs");

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
                .AddReportClientServices(_configuration)
                .AddTransactions();

            services.AddErrorMapper()
                .AddErrorMapper<ApiErrorMapper>();

            services
                .AddAgentApiCore()
                .ConfigureAgentOptions<IConfiguration>((options, config) =>
                {
                    config.GetSection("Agent").Bind(options);
                })
                .ConfigureEmailConfiguration<IConfiguration>((options, config) =>
                {
                    config.GetSection("EmailSetting").Bind(options);
                })
                .AddPaymentServiceClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.PaymentServiceBaseUrl = config.GetValue<Uri>("Agent:PaymentServiceBaseUrl");
                    options.JsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = {new JsonStringEnumConverter()}
                    };
                })
                .ConfigureWallet(config =>
                {
                    config.AddTgsAgentWalletRepositories();
                })
                .AddTgsAgentRepository();

            services.AddHttpClient<IEmailClient, EmailClient>((sp, c) =>
            {
                var url = _configuration.GetSection("Email:Url");
                c.BaseAddress = new(url.Value);
            });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers(options =>
                {
                    options.Filters.Add<CheckAgentEnabledFilter>();
                    options.Filters.Add<ValidateModelFilter>();
                })
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
                    c.SwaggerDoc("v1", new()
                    {
                        Title = "TGS Agent API",
                        Version = "v1"
                    });

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
                    {
                        c.IncludeXmlComments(path);
                    }
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
    }
}