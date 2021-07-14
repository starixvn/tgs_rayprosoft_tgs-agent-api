using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Tgs.Payment.Clients;
using TGS.Services.WalletService.Configuration;

namespace TGS.Services.Promotion.Configuration
{
    public class PromotionConfiguration
    {
        private Dictionary<string, Type> _checkerTypes = new Dictionary<string, Type>();

        public IServiceCollection Services { get; }

        public PromotionConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new System.ArgumentNullException(nameof(services));
        }

        public PromotionConfiguration AddPlayerDataRepository<TPlayerDataRepository>() where TPlayerDataRepository : class, IPlayerDataRepository
        {
            Services.AddTransient<IPlayerDataRepository, TPlayerDataRepository>();

            return this;
        }

        public PromotionConfiguration AddPromotionBonusRepository<TPromotionBonusRepository>() where TPromotionBonusRepository : class, IPromotionBonusRepository
        {
            Services.AddTransient<IPromotionBonusRepository, TPromotionBonusRepository>();

            return this;
        }

        public PromotionConfiguration AddPromotionParticipantRepository<TPromotionParticipantRepository>()
            where TPromotionParticipantRepository : class, IPromotionParticipantRepository
        {
            Services.AddTransient<IPromotionParticipantRepository, TPromotionParticipantRepository>();

            return this;
        }

        public PromotionConfiguration AddPromotionRepository<TPromotionRepository>() where TPromotionRepository : class, IPromotionRepository
        {
            Services.AddTransient<IPromotionRepository, TPromotionRepository>();

            return this;
        }

        public PromotionConfiguration AddSettingRepository<TSettingRepository>() where TSettingRepository : class, ISettingRepository
        {
            Services.AddTransient<ISettingRepository, TSettingRepository>();

            return this;
        }

        public PromotionConfiguration ConfigureWallet(Action<WalletConfiguration> walletConfigure)
        {
            walletConfigure(Services.AddWallet());

            return this;
        }

        public PromotionConfiguration RegisterPromotionChecker<TChecker>(string typeName) where TChecker : class, IPromotionChecker
        {
            Services.AddTransient<TChecker>();

            _checkerTypes[typeName] = typeof(TChecker);

            return this;
        }

        public PromotionConfiguration AddPaymentServiceClient(Action<PaymentServiceClientOptions, IServiceProvider> configure)
        {
            Services.AddPaymentServiceClient(configure);

            return this;
        }

        internal IDictionary<string, Type> GetAllRegisterCheckerTypes() => _checkerTypes;
    }
}
