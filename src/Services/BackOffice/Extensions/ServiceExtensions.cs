using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TGS.Services.BackOffice.Application.Middlewares;
using TGS.Services.BackOffice.Domain;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.BackOffice.Infrastructure.RedisServices;
using TGS.Services.BackOffice.Infrastructure.Services;

namespace TGS.Services.BackOffice.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBackOfficeApplicationServices(this IServiceCollection services)
        {
            services.RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient);
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Singleton);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PipelineBehavior<,>));
            services.AddTransient<IBetLimitService, BetLimitService>();
            services.AddTransient<IBrandService, BrandService>();
            services.AddTransient<IBrandGameService, BrandGameService>();
            services.AddTransient<IBrandGameProviderService, BrandGameProviderService>();
            services.AddTransient<IBrandGameProviderScheduleService, BrandGameProviderScheduleService>();
            services.AddTransient<IGameProviderService, GameProviderService>();
            services.AddTransient<IGameProviderBetLimitService, GameProviderBetLimitService>();
            services.AddTransient<IGameProviderConfigurationService, GameProviderConfigurationService>();
            services.AddTransient<IUploadService, UploadService>();
            services.AddTransient<ICurrencyService, CurrencyService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IVipLevelService, VipLevelService>();
            services.AddTransient<IAgentService, AgentService>();
            services.AddTransient<IAgentBankService, AgentBankService>();
            services.AddTransient<IPlayerService, PlayerService>();
            services.AddTransient<IPlayerBankService, PlayerBankService>();
            services.AddTransient<ICredentialGenerator, CredentialGenerator>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<IWalletExchangeService, WalletExchangeService>();
            services.AddTransient<IPlatformCreditService, PlatformCreditService>();
            services.AddTransient<ISubGameProviderService, SubGameProviderService>();
            services.AddTransient<IMaintenanceRedisService, MaintenanceRedisService>();
            services.AddTransient<IAgentDividendService, AgentDividendService>();
            services.AddTransient<IPlayerIpService, PlayerIpService>();
            services.AddTgsTransactionRepositoriesServices();

            return services;
        }
    }
}