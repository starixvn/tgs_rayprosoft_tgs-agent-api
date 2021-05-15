using System;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using TGS.Clients.Promotion.Client.Extensions;
using TGS.Clients.Report.Client.Extensions;
using TGS.Core.DataServices.Report.Extensions;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Hangfire;
using TGS.Core.Infrastructures.Hangfire.Filters;
using TGS.Core.Infrastructures.HealthChecks;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Operations.Extensions;
using TGS.Shared.Game.ApiClients.Extensions;
using TGS.Workers.Schedulers.Jobs;
using TGS.Workers.Schedulers.Jobs.Agents;
using TGS.Workers.Schedulers.Jobs.Players;
using TGS.Workers.Schedulers.Jobs.Promotion;
using TGS.Workers.Schedulers.Jobs.Reports;
using TGS.Workers.Schedulers.Jobs.Rounds;
using TGS.Workers.Schedulers.Services;
using GlobalConsts = TGS.Core.Models.Consts;

namespace TGS.Workers.Schedulers
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTgsRepository(_configuration, "Tgs")
                .AddReportRepository(_configuration, "Report")
                .AddWebApiServices()
                .AddNats(_configuration)
                .AddTgsLog()
                .AddSystemJsonSerializer()
                .AddRedis()
                .AddTransactions()
                .AddApiClient()
                .AddGameApiClients()
                .AddOperations()
                .AddReportClientServices(_configuration)
                .AddPromotionClientServices(_configuration)
                .AddHangfireScheduler(_configuration, jobProvider: new JobProvider(),
                    typesToUseManagementPage: GetModuleTypes());

            services.AddControllers();

            services.AddGameProvidersMognoRepository((options, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                options.MongoConnectionString = config.GetValue<string>("GameProviders:ConnectionString");
            });

            services.AddHealthChecks()
                .AddPostgres(_configuration, "Tgs", "tgs-db")
                .AddRedis(_configuration)
                .AddMongoDb(_configuration, "Hangfire", "hangfire-mongodb")
                .ForwardToPrometheus();

            // BO services
            services.AddTransient<IBrandGameProviderService, BrandGameProviderService>();

            // job
            services.AddTransient<IPlayerAggregateReportWorkerService, PlayerAggregateReportWorkerService>();
            services.AddTransient<IRoundAggregateWorkerService, RoundAggregateWorkerService>();
            services.AddTransient<IPlayerRebateWorkerService, PlayerRebateWorkerService>();
            services.AddTransient<IAgentDividendWorkerService, AgentDividendWorkerService>();
            services.AddTransient<IPromotionSettlementDailyWorkerService, PromotionSettlementDailyWorkerService>();
            services.AddTransient<IPromotionSettlementWeeklyWorkerService, PromotionSettlementWeeklyWorkerService>();
            services.AddTransient<IPromotionSettlementMonthlyWorkerService, PromotionSettlementMonthlyWorkerService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseGlobalExceptionHandler();

            app.UseHangfireScheduler(new[] { Consts.Queue.Schedule, Consts.Queue.GameProvider, Consts.Queue.Report });

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new()
                {
                    ResultStatusCodes = GlobalConsts.HealthCheckResultStatusCodes,
                    ResponseWriter = (httpContext, result) =>
                        HealthCheckResponseWriter.CreateResponseAsync(httpContext, result, _env)
                });

                endpoints.MapMetrics();

                endpoints.MapHangfireDashboard("/hangfire",
                    new() { Authorization = new[] { new IgnoreHangfireAuthorizationFilter() } });
            });
        }

        private static Type[] GetModuleTypes() =>
            new[] { typeof(JobManagement), typeof(PromotionJobManagement) };
    }
}