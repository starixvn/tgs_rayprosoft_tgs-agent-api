// ReSharper disable CheckNamespace

using TGS.Shared.Game.ApiClients.AllBet;
using TGS.Shared.Game.ApiClients.AsiaGaming;
using TGS.Shared.Game.ApiClients.Avia;
using TGS.Shared.Game.ApiClients.BetConstruct;
using TGS.Shared.Game.ApiClients.BigGaming;
using TGS.Shared.Game.ApiClients.Card365;
using TGS.Shared.Game.ApiClients.Evolution;
using TGS.Shared.Game.ApiClients.IMESports;
using TGS.Shared.Game.ApiClients.JDB;
using TGS.Shared.Game.ApiClients.Jili;
using TGS.Shared.Game.ApiClients.PragmaticPlay;
using TGS.Shared.Game.ApiClients.Saba;
using TGS.Shared.Game.ApiClients.SBOBET;
using TGS.Shared.Game.ApiClients.SexyGaming;
using TGS.Shared.Game.ApiClients.Skywind;
using TGS.Shared.Game.ApiClients.TCG;
using TGS.Shared.Game.ApiClients.TF;
using TGS.Shared.Game.ApiClients.WMCasino;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Dependency Injection of Game.ApiClients, need to add following dependencies beforehand:
    /// 1. AddApiClient() in Infrastructures.ApiClient ( if BaseApiClient is used )
    /// </summary>
    public static IServiceCollection AddGameApiClients(this IServiceCollection services)
    {
        services.AddHttpClient<ISBOApiClient, SBOApiClient>();

        services.AddHttpClient<IPragmaticPlayApiClient, PragmaticPlayApiClient>();

        services.AddHttpClient<IJDBApiClient, JDBApiClient>();

        services.AddHttpClient<IAsiaGamingApiClient, AsiaGamingApiClient>();

        services.AddHttpClient<ISabaApiClient, SabaApiClient>();

        services.AddHttpClient<IIMESportsApiClient, IMESportsApiClient>();

        services.AddHttpClient<IBigGamingApiClient, BigGamingApiClient>();

        services.AddHttpClient<ISexyGamingApiClient, SexyGamingApiClient>();

        services.AddHttpClient<ISkywindApiClient, SkywindApiClient>();

        services.AddHttpClient<IAllBetApiClient, AllBetApiClient>();

        services.AddHttpClient<IEvolutionApiClient, EvolutionApiClient>();

        services.AddHttpClient<IWMCasinoApiClient, WMCasinoApiClient>();

        services.AddHttpClient<ITCGApiClient, TCGApiClient>();
        services.AddScoped<ITCGUtility, TCGUtility>();

        services.AddHttpClient<ICard365ApiClient, Card365ApiClient>();

        services.AddHttpClient<IBetConstructApiClient, BetConstructApiClient>();
        services.AddScoped<IBetConstructUtility, BetConstructUtility>();
        services.AddScoped<IAviaApiClient, AviaApiClient>();

        services.AddHttpClient<IJiliApiClient, JiliApiClient>();

        services.AddHttpClient<ITFApiClient, TFApiClient>();

        return services;
    }
}
