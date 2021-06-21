using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations.Extensions;
using TGS.Shared.Game.Models.Saba.Enums;
using TGS.Shared.Game.Models.Saba.Incoming;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding]
    [Scope(Tag = "Saba")]
    public sealed class SabaEndpoint : BaseGameEndpoint
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Saba;
        private readonly ServiceProvider _serviceProvider;

        public SabaEndpoint(ScenarioContext context)
            : base(context)
        {
            var currentDictionary = Directory.GetCurrentDirectory();

            var services = (IServiceCollection)new ServiceCollection();

#if DEBUG
            var jsonFile = "appsettings.json";
#else
            var jsonFile = "appsettings.IntTest.json";
#endif
            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDictionary)
                .AddJsonFile(jsonFile, true, true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddTgsRepository(configuration, "Tgs")
                .AddSystemJsonSerializer()
                .AddNats(configuration)
                .AddSecurity()
                .AddTgsLog()
                .AddApiClient()
                .AddRedis()
                .AddOperations();

            var isDev = configuration.GetValue<bool>("ApiUrls:IsDev");

            if (isDev)
            {
                services.AddTransient<IPlayerApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<PlayerApiClient>(sp, Hooks.OpApiClient));

                services.AddTransient<ISabaApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<SabaApiClient>(sp, Hooks.TgsApiClient));
            }
            else
            {
                services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Operator")));

                services.AddHttpClient<ISabaApiClient, SabaApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Tgs-Saba")));
            }

            _serviceProvider = services.BuildServiceProvider();
        }

        [Given(@"player ""(.*)"" of brand ""(.*)"" with ""(.*)"" login via ""(.*)""")]
        public Task GivenPlayerOfBrandWithLoginVia(string username, string brandCode,
            string currencyCode, PlatformType platformType) =>
            PlayerLoginAsync(_serviceProvider, username, platformType, brandCode, currencyCode);

        [Given(@"player having balance of \$(.*)")]
        public Task GivenPlayerHavingBalanceOfAsync(decimal balance) =>
            SetPlayerBalanceAsync(_serviceProvider, balance);

        [When(@"I request the player balance")]
        public async Task WhenIRequestThePlayerBalanceAsync()
        {
            var configuration = await GetConfigurationAsync().ConfigureAwait(false);

            var playerCode = Get<string>(SR.PlayerCode);

            var request = new GetBalanceRequest
            {
                Key = configuration.SecretKey,
                Body = new()
                {
                    Action = "getbalance",
                    UserId = playerCode
                }
            };

            var client = _serviceProvider.GetRequiredService<ISabaApiClient>();
            var response = await client.GetBalanceAsync(request).ConfigureAwait(false);

            Set(SR.Response, response);
        }

        [When(@"bet is placed of \$(.*) for game ""(.*)""")]
        public async Task WhenBetIsPlacedOfForGame(decimal amount, string externalGameId)
        {
            var configuration = await GetConfigurationAsync().ConfigureAwait(false);

            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var currencyCode = Get<string>(SR.CurrencyCode);
            var loginRecordId = GetLoginRecordId(playerCode, platform);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var gameProviderCurrencyCode = await db.GameProviderCurrencies
                .AsQueryable()
                .Where(c => c.GameProvider.Code == GameProviderCode && c.CurrencyCode == currencyCode)
                .Select(c => c.GameProviderCurrencyCode).FirstOrDefaultAsync();

            var now = DateTimeOffset.UtcNow.ToIso8601String(true);

            var request = new PlaceBetRequest
            {
                Key = configuration.SecretKey,
                Body = new()
                {
                    Action = "bet",
                    UserId = playerCode,
                    CurrencyCode = Convert.ToInt32(gameProviderCurrencyCode),
                    BetAmount = amount,
                    ActualAmount = amount,
                    BetTime = now,
                    ReferenceId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    Token = loginRecordId.ToString(),
                    UpdateTime = now
                }
            };

            var client = _serviceProvider.GetRequiredService<ISabaApiClient>();
            var response = await client.PlaceBetAsync(request).ConfigureAwait(false);

            var externalRoundId = request.Body.ReferenceId.ToString();

            Set(SR.GameProviderCurrencyCode, gameProviderCurrencyCode);
            Set(SR.ExternalTransactionId, externalRoundId);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.PlaceBetRequest, request);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [Then(@"player balance response will be for \$(.*)")]
        public void ThenPlayerBalanceResponseWillBeFor(decimal expectedBalance)
        {
            var response = Get<GetBalanceResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Status.Should().Be(((int)GameApiError.Success).ToString());
            response.Message.Should().BeNullOrEmpty();
            response.Balance.Should().Be(expectedBalance);
        }

        [Then(@"I will receive place bet response of no error and balance of \$(.*)")]
        public void ThenIWillReceivePlaceBetResponseOfNoErrorAndBalanceOf(decimal expectedBalance)
        {
            var response = Get<PlaceBetResponse>(SR.Response);

            var externalRoundId = Get<string>(SR.ExternalRoundId);

            response.Should().NotBeNull();
            response.Status.Should().Be(((int)GameApiError.Success).ToString());
            response.Message.Should().BeNullOrEmpty();
            response.ReferenceId.ToString().Should().Be(externalRoundId);
        }

        [Then(@"player wallet database record will be for \$(.*)")]
        public async Task ThenPlayerWalletDatabaseRecordWillBeForAsync(decimal expectedBalance)
        {
            var wallet = await GetPlayerWalletAsync(_serviceProvider);
            wallet.Balance.Should().Be(expectedBalance);
        }

        [Then(@"game action will be recorded in the database of type ""(.*)"" for \$(.*)")]
        public async Task ThenGameActionWillBeRecordedInTheDatabaseOfTypeFor(GameActionType type, decimal amount)
        {
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var externalTransactionId = Get<string>(SR.ExternalTransactionId);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var ga = await (
                from act in db.GameActions.AsQueryable()
                join r in db.Rounds on act.RoundId equals r.Id
                where r.ExternalRoundId == externalRoundId &&
                    act.ExternalTransactionId == externalTransactionId
                select act).AsNoTracking().FirstAsync();

            Set(SR.GameAction, ga);

            ga.Should().NotBeNull();
            ga.Type.Should().Be(type);
            ga.Amount.Should().Be(amount);
        }

        #region Private Methods

        private async ValueTask<SabaConfiguration> GetConfigurationAsync() =>
            await GetSettingAsync<SabaConfiguration>(_serviceProvider,
                SettingConsts.GameProviderConfigurations.Saba).ConfigureAwait(false);

        private Guid GetLoginRecordId(string playerCode, PlatformType platform)
        {
            var key = $"{SR.PlayerTokenData}:{playerCode}:{platform}";
            var tokenData = Get<PlayerTokenData>(key);
            return tokenData?.LoginRecordId ?? Guid.Empty;
        }

        #endregion
    }
}