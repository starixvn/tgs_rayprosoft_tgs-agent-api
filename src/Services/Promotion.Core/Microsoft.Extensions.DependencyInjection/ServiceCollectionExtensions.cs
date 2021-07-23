using System.Reflection;
using MediatR;
using TGS.Services.Promotion;
using TGS.Services.Promotion.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static PromotionConfiguration AddPromotionCore(this IServiceCollection services)
        {
            services
                .RegisterAllServiceUtilityTypes(
                    Assembly.GetExecutingAssembly(),
                    ServiceLifetime.Transient)
                .AddMediatR(Assembly.GetExecutingAssembly());

            var configuration = new PromotionConfiguration(services);

            configuration
                .RegisterPromotionChecker<FirstDepositChecker>("FirstDeposit")
                .RegisterPromotionChecker<DailyFirstDepositChecker>("DailyFirstDeposit")
                .RegisterPromotionChecker<DailySecondDepositChecker>("DailySecondDeposit")
                .RegisterPromotionChecker<AlwaysTrueChecker>("TurnoverRebate")
                .RegisterPromotionChecker<AlwaysTrueChecker>("ReliefMoneyDaily")
                .RegisterPromotionChecker<AlwaysTrueChecker>("ReliefMoneyWeekly")
                .RegisterPromotionChecker<AlwaysTrueChecker>("ReliefMoneyMonthly");

            services
                .AddScoped<IPromotionCheckerFactory>(
                    sp => ActivatorUtilities.CreateInstance<PromotionCheckerFactory>(
                        sp,
                        configuration.GetAllRegisterCheckerTypes()))
                .AddTransient<IPlayerService, PlayerService>()
                .AddTransient<ISettingsService, SettingsService>()
                .AddTransient<IPromotionTypeFactory, PromotionTypeFactory>();

            services.AddTgsTransactionRepositoriesServices();

            return configuration;
        }
    }
}