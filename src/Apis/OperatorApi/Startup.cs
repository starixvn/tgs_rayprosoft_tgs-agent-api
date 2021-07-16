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
using Tgs.Payment.ApplePay;
using Tgs.Payment.Clients;
using Tgs.Payment.LeePay;
using Tgs.Payment.PrimePay;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Filters.Swaggers;
using TGS.Apis.OperatorApi.Models;
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
using TGS.Payment.ManualDeposit;
using TGS.Services.Operations.Extensions;
using TGS.Services.Operator.ValidateCode;
using TGS.Services.PluggableServices.Extensions;
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
            app.UseRequestLocalization(options =>
            {
                var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

                options.SupportedCultures.Clear();
                options.SupportedUICultures.Clear();

                config.GetSection("RequestLocalization").Bind(options);
            });

            app.UseTgsErrorHandler();

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

            services.AddNats(_configuration)
                .AddWebApiServices()
                .AddTgsLog()
                .AddRedis()
                .AddSystemJsonSerializer()
                .AddSecurity()
                .AddOperations()
                .AddApiClient()
                .AddPluggableServices()
                .AddReportClientServices(_configuration)
                .AddPromotionClientServices((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    config.GetSection("Promotion").Bind(options);
                })
                .AddTransactions();

            services.AddErrorMapper()
                .AddErrorMapper<OperatorApiErrorMapper>();

            services
                .AddOperatorApiCore()
                .ConfigureOperatorOptions<IConfiguration>((options, config) =>
                {
                    config.GetSection("Operator").Bind(options);
                })
                .UseOperatorRepositories()
                .ConfigureWallet(config =>
                {
                    config.AddTgsWalletRepositories();
                })
                .AddPaymentServiceClient((options, sp) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    options.PaymentServiceBaseUrl = config.GetValue<Uri>("Operator:PaymentServiceBaseUrl");
                    options.JsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = {
                            new JsonStringEnumConverter()
                        }
                    };
                })
                .Services
                .Decorate<IPaymentClient, ManualDepositPaymentClientDecorator>()
                .AddSingleton<PrimePaySerializer>()
                .AddSingleton<ApplePaySerializer>()
                .AddSingleton<LeePaySerializer>();

            services
                .AddOptions<ConstValidateCodeOptions>()
                .Configure<IConfiguration>((options, config) =>
                {
                    config.GetSection("Operator").Bind(options);
                })
                .Services
                .Decorate<IValidateCodeGenerator, ConstValidateCodeGenerator>();

            services
                .AddIdentityVerifiers()
                .AddPlayerBucketRepositoriesServices();

            services
                .AddRouting(options => options.LowercaseUrls = true);
            services
                .AddControllers(options =>
                {
                    options.Filters.Add<ValidateModelFilter>();
                    options.Filters.Add<TgsResponseWrapperAttribute>();
                })
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                .AddFluentValidation(c =>
                {
                    c.RegisterValidatorsFromAssemblyContaining<Startup>(lifetime: ServiceLifetime.Scoped);
                    c.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    c.LocalizationEnabled = false;
                    c.ImplicitlyValidateChildProperties = true;
                });

            services.AddHttpClient<IEmailClient, EmailClient>((sp, c) =>
            {
                var url = _configuration.GetSection("Email:Url");
                c.BaseAddress = new(url.Value);
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
