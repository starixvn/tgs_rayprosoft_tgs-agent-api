using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Extensions;
using TGS.Shared.Game.Models.PragmaticPlay.Common;
using TGS.Shared.Game.Models.PragmaticPlay.Enums;
using TGS.Shared.Game.Models.PragmaticPlay.Incoming;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding]
    [Scope(Tag = "PragmaticPlay")]
    public sealed class PragmaticPlayEndpoint : BaseGameEndpoint
    {
        private static readonly string GameProviderCode = Consts.GameProvider.PragmaticPlay;
        private readonly ServiceProvider _serviceProvider;

        public PragmaticPlayEndpoint(ScenarioContext context)
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

            services.AddSingleton<IPragmaticPlayHashProvider, PragmaticPlayHashProvider>();

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

                services.AddTransient<IPragmaticPlayApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<PragmaticPlayApiClient>(sp, Hooks.TgsApiClient));
            }
            else
            {
                services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Operator")));

                services.AddHttpClient<IPragmaticPlayApiClient, PragmaticPlayApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Tgs-PragmaticPlay")));
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

        [Given(@"player logout")]
        public Task GivenPlayerLogoutAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            return DeauthorizePlayerAsync(_serviceProvider, playerCode);
        }

        [Given(@"bonus code is ""(.*)""")]
        public void GivenBonusCodeIs(string bonusCode) => Set(SR.BonusCode, bonusCode);

        [Given(@"a campaign id is ""(.*)"" and type is ""(.*)""")]
        public void GivenACampaignIdIsAndTypeIs(string campaignId, string campaignType)
        {
            Set(SR.CampaignId, campaignId);
            Set(SR.CampaignType, campaignType);
        }

        [When(@"I call authorize endpoint")]
        public async Task WhenICallAuthorizeEndpoint()
        {
            var configuration = await GetConfigurationAsync();

            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var model = new AuthorizePlayerRequest
            {
                Token = encryptedToken,
                ProviderId = configuration.ProviderId
            };

            var hash = ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var client = _serviceProvider.GetRequiredService<IPragmaticPlayApiClient>();
            var response = await client.AuthorizePlayerAsync(request).ConfigureAwait(false);

            Set(SR.Response, response);
        }

        [When(@"I request the player balance")]
        public async Task WhenIRequestThePlayerBalanceAsync()
        {
            var configuration = await GetConfigurationAsync();

            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var model = new GetBalanceRequest
            {
                Token = encryptedToken,
                ProviderId = configuration.ProviderId,
                UserId = playerCode
            };

            var hash = ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var client = _serviceProvider.GetRequiredService<IPragmaticPlayApiClient>();
            var response = await client.GetBalanceAsync(request).ConfigureAwait(false);

            Set(SR.Response, response);
        }

        [Given(@"I place bet of \$(.*) for game ""(.*)""")]
        [When(@"I place bet of \$(.*) for game ""(.*)""")]
        public async Task WhenIPlaceBetOfForGameAsync(decimal amount, string externalGameId) =>
            await PlaceBetAsync(amount, externalGameId);

        [When(@"I place bet with jackpot of \$(.*)  for game ""(.*)""")]
        public async Task WhenIPlaceBetWithJackpotOfForGame(decimal amount, string externalGameId, Table table)
        {
            var row = table.Rows[0];
            var jackpotContribution =
                new JackpotContribution(row["JackpotId"], decimal.Parse(row["ContributionAmount"]));
            await PlaceBetAsync(amount, externalGameId, jackpotContribution: jackpotContribution);
        }

        [Given(@"I play the bonus for game ""(.*)""")]
        public async Task GivenIPlayTheBonusForGame(string externalGameId)
        {
            var bonusCode = Get<string>(SR.BonusCode);
            await PlaceBetAsync(0, externalGameId, bonusCode);
        }

        [Given(@"I finish all bonus of (.*) rounds and get total win \$(.*) for game ""(.*)""")]
        public async Task GivenIFinishAllBonusOfRoundsAndGetTotalWin(int roundCount, decimal totalAmount,
            string externalGameId)
        {
            var bonusCode = Get<string>(SR.BonusCode);
            var amountOfEachRound = Math.Round(totalAmount / roundCount);

            for (var i = 0; i < roundCount; i++)
            {
                var amount = i == roundCount - 1
                    ? totalAmount - amountOfEachRound * i
                    : amountOfEachRound;

                await PlaceBetAsync(0, externalGameId, bonusCode);
                await WinBetAsync(amount, bonusCode);
            }

            Set(SR.Amount, totalAmount);
        }

        [Given(@"I win bet for \$(.*)")]
        [When(@"I win bet for \$(.*)")]
        public async Task WhenIWinBetForAsync(decimal amount) =>
            await WinBetAsync(amount);

        [When(@"I win the bonus of \$(.*)")]
        public async Task WhenIWinTheBonusOf(decimal amount)
        {
            var bonusCode = Get<string>(SR.BonusCode);
            await WinBetAsync(amount, bonusCode);
        }

        [When(@"I win prize drop event of \$(.*)")]
        public async Task WhenIWinPrizeDropEventOf(decimal prizeAmount)
        {
            var configuration = await GetConfigurationAsync();

            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var externalGameId = Get<string>(SR.ExternalGameId);
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var externalTransactionId = Guid.NewGuid().ToString();
            var promoTransactionId = Guid.NewGuid().ToString();

            var model = new ResultRequest
            {
                Token = encryptedToken,
                ProviderId = configuration.ProviderId,
                UserId = playerCode,
                Amount = 0,
                GameId = externalGameId,
                Reference = externalTransactionId,
                RoundId = externalRoundId,
                RoundDetails = string.Empty,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                PromoCampaignId = "PrizeDropEvent",
                PromoWinAmount = prizeAmount,
                PromoWinReference = promoTransactionId,
                PromoCampaignType = "R"
            };

            var hash = ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var client = _serviceProvider.GetRequiredService<IPragmaticPlayApiClient>();
            var response = await client.ResultAsync(request).ConfigureAwait(false);

            Set(SR.ExternalTransactionId, promoTransactionId);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [Given(@"I lose bet")]
        [When(@"I lose bet")]
        public async Task WhenILoseBetAsync()
        {
            var configuration = await GetConfigurationAsync();

            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var externalGameId = Get<string>(SR.ExternalGameId);
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var externalTransactionId = Get<string>(SR.ExternalTransactionId);

            var model = new EndRoundRequest
            {
                Token = encryptedToken,
                ProviderId = configuration.ProviderId,
                UserId = playerCode,
                GameId = externalGameId,
                RoundId = externalRoundId
            };

            var hash = ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var client = _serviceProvider.GetRequiredService<IPragmaticPlayApiClient>();
            var response = await client.EndRoundAsync(request).ConfigureAwait(false);

            Set(SR.ExternalTransactionId, $"{externalTransactionId}_lose");
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [Given(@"I cancel transaction")]
        [When(@"I cancel transaction")]
        public async Task WhenICancelTransaction()
        {
            var configuration = await GetConfigurationAsync();

            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var betRequest = Get<PlaceBetRequest>(SR.PlaceBetRequest);

            var model = new RefundRequest
            {
                Token = encryptedToken,
                ProviderId = configuration.ProviderId,
                UserId = playerCode,
                GameId = betRequest.GameId,
                RoundId = betRequest.RoundId,
                Reference = betRequest.Reference,
                Amount = betRequest.Amount,
                RoundDetails = string.Empty,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            var hash = ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var client = _serviceProvider.GetRequiredService<IPragmaticPlayApiClient>();
            var response = await client.RefundAsync(request).ConfigureAwait(false);

            Set(SR.ExternalTransactionId, $"{betRequest.Reference}_cancel");
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [When(@"I win jackpot of \$(.*) for game ""(.*)""")]
        public async Task WhenIWinJackpotOfForGame(decimal amount, string externalGameId)
        {
            var configuration = await GetConfigurationAsync();

            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var externalRoundId = Guid.NewGuid().ToString();
            var externalTransactionId = Guid.NewGuid().ToString();

            var model = new JackpotWinRequest
            {
                Token = encryptedToken,
                ProviderId = configuration.ProviderId,
                UserId = playerCode,
                Amount = amount,
                GameId = externalGameId,
                Reference = externalTransactionId,
                RoundId = externalRoundId,
                JackpotId = externalTransactionId,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            var hash = ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var client = _serviceProvider.GetRequiredService<IPragmaticPlayApiClient>();
            var response = await client.WinJackpotAsync(request).ConfigureAwait(false);

            Set(SR.ExternalRoundId, $"{externalRoundId}_jackpot");
            Set(SR.ExternalTransactionId, externalTransactionId);
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.PlaceBetRequest, request);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [When(@"bet win request is triggered after the bonus are finished")]
        public async Task WhenBetWinRequestIsTriggeredAfterTheBonusAreFinished()
        {
            var configuration = await GetConfigurationAsync();

            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var bonusCode = Get<string>(SR.BonusCode);
            var amount = Get<decimal>(SR.Amount);
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var externalGameId = Get<string>(SR.ExternalGameId);
            var externalTransactionId = Guid.NewGuid().ToString();

            var model = new BonusWinRequest
            {
                Token = encryptedToken,
                ProviderId = configuration.ProviderId,
                UserId = playerCode,
                Amount = amount,
                GameId = externalGameId,
                Reference = externalTransactionId,
                RoundId = externalRoundId,
                BonusCode = bonusCode,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            var hash = ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var client = _serviceProvider.GetRequiredService<IPragmaticPlayApiClient>();
            var response = await client.WinBonusAsync(request).ConfigureAwait(false);

            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [When(@"I win the campaign of \$(.*)")]
        public async Task WhenIWinTheCampaignOf(decimal amount)
        {
            var configuration = await GetConfigurationAsync();

            var playerCode = Get<string>(SR.PlayerCode);
            var campaignId = Get<string>(SR.CampaignId);
            var campaignType = Get<string>(SR.CampaignType);
            var currency = Get<string>(SR.CurrencyCode);

            var externalRoundId = Guid.NewGuid().ToString();

            var model = new PromoWinRequest
            {
                ProviderId = configuration.ProviderId,
                UserId = playerCode,
                Amount = amount,
                Reference = externalRoundId,
                CampaignId = campaignId,
                CampaignType = campaignType,
                Currency = currency,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            var hash = ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var client = _serviceProvider.GetRequiredService<IPragmaticPlayApiClient>();
            var response = await client.WinPromoAsync(request).ConfigureAwait(false);

            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.ExternalTransactionId, externalRoundId);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [Then(@"I will get authorize response and player balance will be \$(.*)")]
        public async Task ThenIWillGetAuthorizeResponseAndPlayerBalanceWillBe(decimal balance)
        {
            var wallet = await GetPlayerWalletAsync(_serviceProvider);

            var gameProviderOperations = _serviceProvider.GetRequiredService<IGameProviderOperations>();
            var gameProviderId = await gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderCode)
                .ConfigureAwait(false);

            var mapper = _serviceProvider.GetRequiredService<IGameProviderCurrencyCodeMapper>();
            var gameProviderCurrency = await mapper.InternalToExternalAsync(wallet.CurrencyCode, gameProviderId)
                .ConfigureAwait(false);

            var response = Get<AuthorizePlayerResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Balance.Should().Be(balance);
            response.Currency.Should().Be(gameProviderCurrency);
        }

        [Then(@"I will get error authorize response and no player balance")]
        public void ThenIWillGetErrorAuthorizeResponseAndNoPlayerBalance()
        {
            var response = Get<AuthorizePlayerResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Error.Should().NotBe(GameApiError.Success);
            response.Balance.Should().Be(0);
        }

        [Then(@"player balance response will be for \$(.*)")]
        public void ThenPlayerBalanceResponseWillBeFor(decimal expectedBalance)
        {
            var response = Get<GetBalanceResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Error.Should().Be(GameApiError.Success);
            response.Balance.Should().Be(expectedBalance);
        }

        [Then(@"I will receive bet response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveBetResponseOfNoErrorAndBalanceOf(decimal expectedBalance)
        {
            var response = Get<PlaceBetResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Error.Should().Be(GameApiError.Success);
            response.Balance.Should().Be(expectedBalance);
            response.TransactionId.Should().NotBeNull();
        }

        [Then(@"I will receive result response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveResultResponseOfNoErrorAndBalanceOf(decimal expectedBalance)
        {
            var response = Get<ResultResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Error.Should().Be(GameApiError.Success);
            response.Balance.Should().Be(expectedBalance);
            response.TransactionId.Should().NotBeNull();
        }

        [Then(@"I will receive endRound response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveEndRoundResponseOfNoErrorAndBalanceOf(decimal expectedBalance)
        {
            var response = Get<EndRoundResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Error.Should().Be(GameApiError.Success);
            response.Balance.Should().Be(expectedBalance);
        }

        [Then(@"I will receive refund response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveRefundResponseOfNoErrorAndBalanceOf(decimal expectedBalance)
        {
            var response = Get<RefundResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Error.Should().Be(GameApiError.Success);
            response.TransactionId.Should().NotBeNull();
        }

        [Then(@"I will receive jackpot win response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveJackpotWinResponseOfNoErrorAndBalanceOf(decimal expectedBalance)
        {
            var response = Get<JackpotWinResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Error.Should().Be(GameApiError.Success);
            response.TransactionId.Should().NotBeNull();
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

        [Then(@"jackpot contribute data in game action is as set value")]
        public void ThenJackpotContributeDataInGameActionIsAsSetValue()
        {
            var jackpotContribution = Get<JackpotContribution>(SR.JackpotContribution);
            var ga = Get<GameAction>(SR.GameAction);

            var json = _serviceProvider.GetRequiredService<IJsonSerializationProvider>();
            var contrib =
                json.DeserializeFromString<Core.Models.Services.Games.JackpotContribution>(ga.JackpotContribution);

            contrib.Should().NotBeNull();
            contrib.JackpotId.Should().Be(jackpotContribution.Id);
            contrib.TotalAmount.Should().Be(jackpotContribution.Amount);
        }

        [Then(@"bonus code in round detail is as set value")]
        public async Task ThenBonusCodeInRoundDetailIsAsSetValue()
        {
            var bonusCode = Get<string>(SR.BonusCode);
            var ga = Get<GameAction>(SR.GameAction);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var roundDetail = await (
                from detail in db.RoundDetails.AsQueryable()
                where detail.RoundId == ga.RoundId
                select detail.AdditionalData).AsNoTracking().FirstAsync();

            var json = _serviceProvider.GetRequiredService<IJsonSerializationProvider>();
            var data = json.DeserializeFromString<Dictionary<string, string>>(roundDetail);

            data["BonusCode"].Should().Be(bonusCode);
        }

        [Then(@"campaign data in round detail is as set value")]
        public async Task ThenCampaignDataInRoundDetailIsAsSetValue()
        {
            var campaignId = Get<string>(SR.CampaignId);
            var type = Get<string>(SR.CampaignType);
            var ga = Get<GameAction>(SR.GameAction);

            PragmaticPlayConsts.CampaignTypeMap.TryGetValue(type, out var campaignType);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var roundDetail = await (
                from detail in db.RoundDetails.AsQueryable()
                where detail.RoundId == ga.RoundId
                select detail.AdditionalData).AsNoTracking().FirstAsync();

            var json = _serviceProvider.GetRequiredService<IJsonSerializationProvider>();
            var data = json.DeserializeFromString<Dictionary<string, string>>(roundDetail);

            data["CampaignId"].Should().Be(campaignId);
            data["CampaignType"].Should().Be(campaignType);
        }

        [Then(@"I will receive bet win response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveBetWinResponseOfNoErrorAndBalanceOf(decimal expectedBalance)
        {
            var response = Get<BonusWinResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Balance.Should().Be(expectedBalance);
        }

        [Then(@"I will receive promo win response of no error and balance of \$(.*)")]
        public void ThenIWillReceivePromoWinResponseOfNoErrorAndBalanceOf(decimal expectedBalance)
        {
            var response = Get<PromoWinResponse>(SR.Response);

            response.Should().NotBeNull();
            response.TransactionId.Should().NotBeNullOrEmpty();
            response.Balance.Should().Be(expectedBalance);
        }

        #region After Scenario

        [After]
        public void AfterBonusWinScenarioAsync()
        {
            if (!HasTag("BonusWin"))
            {
                return;
            }

            Remove(SR.BonusCode);
        }

        [After]
        public void AfterPromoWinScenarioAsync()
        {
            if (!HasTag("PromoWin"))
            {
                return;
            }

            Remove(SR.CampaignId);
            Remove(SR.CampaignType);
        }

        #endregion After Scenario

        #region Private Members

        private async ValueTask<PragmaticPlayConfiguration> GetConfigurationAsync() =>
            await GetSettingAsync<PragmaticPlayConfiguration>(_serviceProvider,
                SettingConsts.GameProviderConfigurations.PragmaticPlay).ConfigureAwait(false);

        private string ComputeHash<T>(T model, string secretKey) where T : HashRequest
        {
            var provider = _serviceProvider.GetRequiredService<IPragmaticPlayHashProvider>();
            return provider.ComputeHash(model, secretKey);
        }

        private Task<string> GetEncryptedTokenAsync(string playerCode, PlatformType platform)
        {
            var key = $"{SR.PlayerTokenData}:{playerCode}:{platform}";
            var tokenData = Get<PlayerTokenData>(key);
            var loginRecordId = tokenData?.LoginRecordId ?? Guid.Empty;

            var tokenProvider = _serviceProvider.GetRequiredService<ITokenProvider>();
            return tokenProvider.EncryptAsync(loginRecordId);
        }

        private async ValueTask PlaceBetAsync(decimal amount, string externalGameId, string bonusCode = null,
            JackpotContribution jackpotContribution = null)
        {
            var configuration = await GetConfigurationAsync();

            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var externalRoundId = Guid.NewGuid().ToString();
            var externalTransactionId = Guid.NewGuid().ToString();

            var model = new PlaceBetRequest
            {
                Token = encryptedToken,
                ProviderId = configuration.ProviderId,
                UserId = playerCode,
                Amount = amount,
                GameId = externalGameId,
                Reference = externalTransactionId,
                RoundId = externalRoundId,
                RoundDetails = string.Empty,
                BonusCode = bonusCode,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                JackpotId = jackpotContribution?.Id,
                JackpotContribution = jackpotContribution?.Amount
            };

            var hash = ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var client = _serviceProvider.GetRequiredService<IPragmaticPlayApiClient>();
            var response = await client.PlaceBetAsync(request).ConfigureAwait(false);

            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.ExternalTransactionId, externalTransactionId);
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.PlaceBetRequest, request);
            Set(SR.Request, request);
            Set(SR.Response, response);

            if (jackpotContribution != null)
            {
                Set(SR.JackpotContribution, jackpotContribution);
            }
        }

        private async ValueTask WinBetAsync(decimal amount, string bonusCode = null)
        {
            var configuration = await GetConfigurationAsync();

            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var externalGameId = Get<string>(SR.ExternalGameId);
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var externalTransactionId = Guid.NewGuid().ToString();

            var model = new ResultRequest
            {
                Token = encryptedToken,
                ProviderId = configuration.ProviderId,
                UserId = playerCode,
                Amount = amount,
                GameId = externalGameId,
                Reference = externalTransactionId,
                RoundId = externalRoundId,
                RoundDetails = string.Empty,
                BonusCode = bonusCode,
                Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            var hash = ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var client = _serviceProvider.GetRequiredService<IPragmaticPlayApiClient>();
            var response = await client.ResultAsync(request).ConfigureAwait(false);

            Set(SR.ExternalTransactionId, externalTransactionId);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        private record JackpotContribution(string Id, decimal Amount);

        #endregion Private Members
    }
}