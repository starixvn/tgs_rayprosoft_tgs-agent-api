// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TGS.Core.DataServices.Base.Extensions;
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
using TGS.Shared.Game.Models.SexyGaming.Enums;
using TGS.Shared.Game.Models.SexyGaming.Incoming;
using TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding]
    [Scope(Tag = "SexyGaming")]
    public class SexyGamingEndpoint : BaseGameEndpoint
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly ISexyGamingApiClient _client;

        private readonly IHashProvider _hashProvider;

        public SexyGamingEndpoint(ScenarioContext context)
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

                services.AddTransient<ISexyGamingApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<SexyGamingApiClient>(sp, Hooks.TgsApiClient));
            }
            else
            {
                services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Operator")));

                services.AddHttpClient<ISexyGamingApiClient, SexyGamingApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Tgs-SexyGaming")));
            }

            _serviceProvider = services.BuildServiceProvider();
            _client = _serviceProvider.GetRequiredService<ISexyGamingApiClient>();
            _hashProvider = _serviceProvider.GetRequiredService<IHashProvider>();
        }
        [Given(@"player ""(.*)"" of brand ""(.*)"" with ""(.*)"" login via ""(.*)""")]
        public Task GivenPlayerOfBrandWithLoginViaAsync(string username, string brandCode,
            string currencyCode, PlatformType platformType) =>
            PlayerLoginAsync(_serviceProvider, username, platformType, brandCode, currencyCode);


        [Given(@"player having balance of \$(.*)")]
        public Task GivenPlayerHavingBalanceOfAsync(decimal balance) =>
            SetPlayerBalanceAsync(_serviceProvider, balance);

        [Then(@"player balance response will be for \$(.*)")]
        public void ThenPlayerBalanceResponseWillBeFor(decimal expectedBalance)
        {
            var response = Get<SexyGamingGetBalanceResponse>(SR.Response);

            decimal balance = JsonSerializer.Deserialize<decimal>(JsonSerializer.Serialize(response.Balance));
            response.Status.Should().Be("0000");
            balance.Should().Be(expectedBalance);
        }


        [When(@"I request the player balance")]
        public async Task WhenIRequestThePlayerBalanceAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);

            var configuration = await GetConfigurationAsync().ConfigureAwait(false);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);
            var data = new SexyGamingGetBalanceRequest()
            {
                Action = "getBalance",
                UserId = playerCode
            };

            var response = await _client.GetPlayerBalanceAsync(configuration.Cert, encryptedToken, data).ConfigureAwait(false);

            Set(SR.Response, response);
        }


        [When(@"I place bet of \$(.*) for game ""(.*)""")]
        public async Task WhenIPlaceBetOfForGameAsync(decimal amount, string externalGameId) =>
            await PlaceBetAsync(amount, externalGameId);

        [Then(@"I will receive bet response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveBetResponseOfNoErrorAndBalanceOf(decimal expectedBalance)
        {
            var response = Get<SexyGamingGetBalanceResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Status.Should().Be(((int)GameApiError.Success).ToString("d4"));
            response.Balance.Should().Be(expectedBalance);
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
                where r.ExternalRoundId == externalRoundId && act.ExternalTransactionId == externalTransactionId
                select act).AsNoTracking().FirstAsync();

            Set(SR.GameAction, ga);

            ga.Should().NotBeNull();
            ga.Type.Should().Be(type);
            ga.Amount.Should().Be(amount);
        }


        [Given(@"I place bet of \$(.*) for game ""(.*)""")]
        public async Task GivenIPlaceBetOfForGameAsync(decimal amount, string externalGameId) =>
           await PlaceBetAsync(amount, externalGameId);

        [Given(@"balance now being \$(.*)")]
        public void GivenBalanceNowBeing(decimal expectedBalance)
        {
            var response = Get<SexyGamingGetBalanceResponse>(SR.Response);
            response.Should().NotBeNull();
            response.Status.Should().Be(((int)GameApiError.Success).ToString("d4"));
            response.Balance.Should().Be(expectedBalance);
        }
        [Given(@"player logout")]
        public Task GivenPlayerLogoutAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            return DeauthorizePlayerAsync(_serviceProvider, playerCode);
        }

        [When(@"I win bet for \$(.*)")]
        public async Task WhenIWinBetForAsync(decimal amount)
        {
            var externalGameId = Get<string>(SR.ExternalGameId);
            await SettleBetAsync(GameActionType.WinBet, amount, externalGameId);
        }

        [Then(@"I will receive settle bet response of no error and balance of \$(.*)")]
        public async Task ThenIWillReceiveSettleBetResponseOfNoErrorAndBalanceOfAsync(decimal expectedBalance)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);

            var configuration = await GetConfigurationAsync().ConfigureAwait(false);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);
            var data = new SexyGamingGetBalanceRequest()
            {
                Action = "getBalance",
                UserId = playerCode
            };

            var response = await _client.GetPlayerBalanceAsync(configuration.Cert, encryptedToken, data).ConfigureAwait(false);

            response.Should().NotBeNull();
            response.Status.Should().Be(((int)GameApiError.Success).ToString("d4"));
            response.Balance.Should().Be(expectedBalance);
        }


        [Then(@"the round will be closed")]
        public async void ThenTheRoundWillBeClosed() =>
        await VerifyRoundStatus(_serviceProvider, RoundStatus.Closed);


        #region Private Methods

        private static string NewRoundId() =>
           Guid.NewGuid().ToString();

        private async ValueTask<SexyGamingConfiguration> GetConfigurationAsync() =>
            await GetSettingAsync<SexyGamingConfiguration>(_serviceProvider,
                SettingConsts.GameProviderConfigurations.SexyGaming).ConfigureAwait(false);

        private Guid GetLoginRecordId(string playerCode, PlatformType platform)
        {
            var key = $"{SR.PlayerTokenData}:{playerCode}:{platform}";
            var tokenData = Get<PlayerTokenData>(key);
            return tokenData?.LoginRecordId ?? Guid.Empty;
        }

        private Task<string> GetEncryptedTokenAsync(string playerCode, PlatformType platform)
        {
            var key = $"{SR.PlayerTokenData}:{playerCode}:{platform}";
            var tokenData = Get<PlayerTokenData>(key);
            var loginRecordId = tokenData?.LoginRecordId ?? Guid.Empty;

            var tokenProvider = _serviceProvider.GetRequiredService<ITokenProvider>();
            return tokenProvider.EncryptAsync(loginRecordId);
        }

        private async Task PlaceBetAsync(decimal amount, string externalGameId, string externalRoundId = null)
        {
            var currencyCode = Get<string>(SR.CurrencyCode);
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var configuration = await GetConfigurationAsync().ConfigureAwait(false);
            externalRoundId ??= NewRoundId();
            var externalTransactionId = externalRoundId;
            var client = _serviceProvider.GetRequiredService<ISexyGamingApiClient>();
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);
            var transactions = new SexyGamingBet[] {
             new SexyGamingBet(){
                 PlatformTxId =externalRoundId,
                 UserId =playerCode,
                 Currency =currencyCode,
                 Platform =platform.ToString(),
                 GameType ="SEXYBCRT",
                 GameCode=externalGameId,
                 GameName=externalGameId,
                 BetType="Banker",
                 BetAmount =amount,
                 BetTime =DateTimeOffset.UtcNow,
                 RoundId = externalRoundId,
                 GameInfo =null}
            };


            var request = new SexyGamingSingleWalletTxnsRequest<SexyGamingBet>
            {
                Action = "bet",
                Transaction = transactions

            };

            var response = await client.PlaceBetAsync(configuration.Cert, encryptedToken, request);

            Set(SR.ExternalTransactionId, externalTransactionId);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.PlaceBetRequest, request);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        private async Task SettleBetAsync(GameActionType type, decimal amount, string externalGameId = null)
        {
            var currencyCode = Get<string>(SR.CurrencyCode);
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var configuration = await GetConfigurationAsync();
            var db = GetDb<ITgsRepository>(_serviceProvider);
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var externalTransactionId = Get<string>(SR.ExternalTransactionId);

            var client = _serviceProvider.GetRequiredService<ISexyGamingApiClient>();


            var ga = await (
              from act in db.GameActions.AsQueryable()
              join r in db.Rounds on act.RoundId equals r.Id
              where r.ExternalRoundId == externalRoundId && act.ExternalTransactionId == externalTransactionId
              select act).AsNoTracking().FirstAsync();

            var transactions = new SexyGamingSettle[] {
             new SexyGamingSettle(){
                 PlatformTxId =$"{ga.ExternalTransactionId}",
                 UserId =playerCode,
                 Platform =platform.ToString(),
                 GameType ="SEXYBCRT",
                 GameCode=externalGameId,
                 GameName=externalGameId,
                 BetType=ga.BetType,
                 BetAmount =ga.Amount,
                 BetTime =DateTimeOffset.Parse(ga.ExtraData),
                 WinAmount=amount,
                 UpdateTime=DateTimeOffset.UtcNow,
                 Turnover=ga.Amount,
                 RoundId = externalRoundId,
                 GameInfo =null,
                 TxTime=DateTimeOffset.UtcNow
             }
            };


            var request = new SexyGamingSingleWalletTxnsRequest<SexyGamingSettle>
            {
                Action = "settle",
                Transaction = transactions

            };

            var response = await client.SettleBetAsync(configuration.Cert, request);

            var settleId = $"{externalTransactionId}_settle";

            Set(SR.ExternalTransactionId, settleId);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        #endregion
    }
}
