using System;
using Microsoft.Extensions.DependencyInjection;
using Tgs.Payment.Clients;
using TGS.Core.Infrastructures.Mail;
using TGS.Core.Infrastructures.Redis;
using TGS.Services.WalletService.Configuration;

namespace TGS.Services.Agent.Configuration;

public class AgentConfiguration(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));

    public AgentConfiguration AddAgentLoginRecordRepository<TAgentLoginRecordRepository>()
        where TAgentLoginRecordRepository : class, IAgentLoginRecordRepository
    {
        _ = Services.AddTransient<IAgentLoginRecordRepository, TAgentLoginRecordRepository>()
            .Decorate<IAgentLoginRecordRepository>((innerRepository, sp)
                =>
            {
                var redisDatabase = sp
                    .GetRequiredService<IRedisConnectionFactory>()
                    .GetConnection()
                    .GetDatabase();

                return ActivatorUtilities.CreateInstance<RedisAgentLoginRecordRepository>(sp, innerRepository, redisDatabase);
            })
            .Decorate<IAgentLoginRecordRepository, MemoryCachedAgentLoginRecordRepository>();

        return this;
    }

    public AgentConfiguration AddAgentRepository<TAgentRepository>() where TAgentRepository : class, IAgentRepository
    {
        _ = Services.AddTransient<IAgentRepository, TAgentRepository>();

        return this;
    }

    public AgentConfiguration AddBrandRepository<TBrandRepository>() where TBrandRepository : class, IBrandRepository
    {
        _ = Services.AddTransient<IBrandRepository, TBrandRepository>();

        return this;
    }

    public AgentConfiguration AddGameProviderRepository<TGameProviderRepository>() where TGameProviderRepository : class, IGameProviderRepository
    {
        _ = Services.AddTransient<IGameProviderRepository, TGameProviderRepository>();

        return this;
    }

    public AgentConfiguration AddMaintenanceRedisRepository<TMaintenanceRedisRepository>()
                where TMaintenanceRedisRepository : class, IMaintenanceRedisRepository
    {
        Services
            .AddTransient<IMaintenanceRedisRepository, TMaintenanceRedisRepository>();

        return this;
    }

    public AgentConfiguration AddPaymentServiceClient(Action<IHttpClientBuilder> httpClientConfigure)
    {
        _ = Services.AddPaymentServiceClient(httpClientConfigure);

        return this;
    }

    public AgentConfiguration ConfigureAgentOptions<TDep>(Action<AgentOptions, TDep> configure) where TDep : class
    {
        _ = Services
            .AddOptions<AgentOptions>()
            .Configure(configure);

        return this;
    }

    public AgentConfiguration ConfigureWallet(Action<WalletConfiguration> configure)
    {
        configure(Services.AddWallet());

        return this;
    }
}
