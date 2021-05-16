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
using TGS.Apis.TgsApi.GameProviders.IMESports;
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
using TGS.Shared.Game.Models.IMESports.Common;
using TGS.Shared.Game.Models.IMESports.Enums;
using TGS.Shared.Game.Models.IMESports.Incoming;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;
using TGS.Tests.TestsCommon.Extensions;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding]
    [Scope(Tag = "IMESports")]
    public sealed class IMESportsEndpoint : BaseGameEndpoint
    {
        private readonly IIMESportsApiClient _client;
        private readonly ServiceProvider _serviceProvider;
        private const string SessionToken = "asdfgqwer0987";
        private const string Valid = "valid";
        private const string Prefix = "tgs";

        public IMESportsEndpoint(ScenarioContext scenarioContext)
            : base(scenarioContext)
        {
            var services = (IServiceCollection)new ServiceCollection();
#if DEBUG
            var jsonFile = "appsettings.json";
#else
            var jsonFile = "appsettings.IntTest.json";
#endif

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
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
                .AddRedis();

            var apiUrlSection = configuration.GetSection("ApiUrls");
            var isDev = configuration.GetValue<bool>("ApiUrls:IsDev");

            if (isDev)
            {
                services.AddTransient<IPlayerApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<PlayerApiClient>(sp, Hooks.OpApiClient));
                services.AddTransient<IIMESportsApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<IMESportsApiClient>(sp, Hooks.TgsApiClient));
            }
            else
            {
                services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(c =>
                    c.BaseAddress = new(apiUrlSection.GetValue<string>("Operator")));

                services.AddHttpClient<IIMESportsApiClient, IMESportsApiClient>(c =>
                    c.BaseAddress = new(apiUrlSection.GetValue<string>("Tgs-IMESports")));
            }
            _serviceProvider = services.BuildServiceProvider();
            _client = _serviceProvider.GetRequiredService<IIMESportsApiClient>();
        }

        [Given(@"player ""(.*)"" of brand ""(.*)"" with ""(.*)"" login via ""(.*)""")]
        public Task GivenPlayerOfBrandLoginViaAsync(string username, string brandCode,
            string currencyCode, PlatformType platformType) =>
            PlayerLoginAsync(_serviceProvider, username, platformType, brandCode, currencyCode);

        [Given(@"player logout")]
        public Task GivenPlayerLogoutAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            return DeauthorizePlayerAsync(_serviceProvider, playerCode);
        }

        [Given(@"player ""(.*)"" of brand ""(.*)"" without login")]
        public async Task GivenPlayerOfBrandWithoutLoginAsync(string username, string brandCode)
        {
            var db = GetDb<ITgsRepository>(_serviceProvider);
            var playerCode = await
                (from p in db.Players.AsQueryable()
                 join b in db.Brands on p.BrandId equals b.Id
                 where p.Username == username && b.Code == brandCode
                 select p.PlayerCode).AsNoTracking().FirstAsync();

            await DeauthorizePlayerAsync(_serviceProvider, playerCode);

            Set(SR.BrandCode, brandCode);
            Set(SR.PlayerCode, playerCode);
        }

        [Given(@"add (.*) session token in redis for game ""(.*)""")]
        public Task GivenAddValidSessionTokenInRedisForGameAsync(string isValid, string gameId)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var timeToLeave = 180;
            var cacheManager = _serviceProvider.GetRequiredService<IRedisCacheManager>();
            var redisKey = $"{playerCode}_{gameId}";

            if (isValid.IgnoreCaseNotEquals(Valid))
            {
                return cacheManager.ClearCacheAsync(IMESportsConsts.RedisKey.SessionToken, redisKey);
            }

            return cacheManager.SafeCacheAsync(IMESportsConsts.RedisKey.SessionToken,
                redisKey,
                TimeSpan.FromSeconds(timeToLeave), new IMSessionToken
                {
                    SessionToken = SessionToken,
                    TimeToLeave = timeToLeave
                });
        }

        [Given(@"I place bet of \$(.*) for game ""(.*)""")]
        [When(@"I place bet of \$(.*) for game ""(.*)""")]
        public Task WhenIPlaceBetOfForGameAsync(decimal amount, string externalGameId) =>
            PlaceBetAsync(amount, externalGameId);

        [When(@"I place bet of \$(.*) for game ""(.*)"" with invalid product wallet")]
        public Task WhenIPlaceBetOfForGameWithInvalidProductWallet(decimal amount, string externalGameId) =>
            PlaceBetAsync(amount, externalGameId, validProductWallet: false);

        [Given(@"players place bets:")]
        public async Task GivenPlayersPlaceBets(Table table)
        {
            var players = table.CreateList<PlayerData>().ToArray();
            var betRequests = new List<BetRequest>();

            foreach (var p in players)
            {
                await GivenPlayerOfBrandLoginViaAsync(p.Username, p.Brand, p.CurrencyCode, p.Platform);
                await GivenPlayerHavingBalanceOfAsync(p.Balance);

                p.PlayerCode = Get<string>(SR.PlayerCode);
                await GivenAddValidSessionTokenInRedisForGameAsync(Valid, p.GameId);

                await WhenIPlaceBetOfForGameAsync(p.Amount, p.GameId);

                var request = Get<BetRequest>(SR.Request);
                betRequests.Add(request);
            }

            Set(SR.Requests, betRequests);
            Set(SR.Players, players.ToDictionary(p => p.Username, p => p));
        }

        [Given(@"I use action (.*) to bet for \$(.*)")]
        [When(@"I use action (.*) to bet for \$(.*)")]
        public Task WhenIUseActionToBetFor(ActionType actionType, decimal winAmount) =>
            SettleAndCancelBetAsync(winAmount, actionType);

        [When(@"I use action to bets")]
        public async Task WhenIUseActionToBets(Table table)
        {
            var players = table.CreateList<PlayerData>();
            var betPlayers = Get<Dictionary<string, PlayerData>>(SR.Players);
            var betRequests = Get<List<BetRequest>>(SR.Requests);
            var betTransactions = betRequests.SelectMany(b => b.Transactions);

            var transactions = new List<TransactionRequest>();
            foreach (var player in players)
            {
                var playerCode = $"{Prefix}_{betPlayers[player.Username].PlayerCode}";
                player.PlayerCode = playerCode;
                var betTransaction = betTransactions.First(t => t.PlayerCode == playerCode);
                transactions.Add(GetTransactionRequest(player, betTransaction.ExternalRoundId, Guid.NewGuid().ToString()));
            }

            var response = await SubmitSettleAndCancelBetAsync(transactions.ToArray());
            Set(SR.Response, response);
        }

        [When(@"I resend the settle bet")]
        public Task WhenIResendTheWinBetAsync() =>
            ResentSettleBetAsync();

        [Given(@"player having balance of \$(.*)")]
        public Task GivenPlayerHavingBalanceOfAsync(decimal balance) =>
            SetPlayerBalanceAsync(_serviceProvider, balance);

        [When(@"I request the player balance")]
        public async Task WhenIRequestThePlayerBalanceAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var currency = Get<string>(SR.CurrencyCode);
            var config = await GetConfigurationAsync();

            var response = await _client.GetPlayerBalanceAsync(new()
            {
                ProductWallet = GetProductWallet(config),
                PlayerCode = $"{Prefix}_{playerCode}",
                Currency = currency
            });

            Set(SR.Response, response);
        }

        [Then(@"player balance response will be for \$(.*)")]
        public void ThenPlayerBalanceResponseWillBeFor(decimal expectedBalance)
        {
            var response = Get<GetBalanceResponse>(SR.Response);

            response.Should().NotBeNull();
            response.ErrorCode.Should().Be((int)GameApiError.Success);
            response.Balance.Should().Be(expectedBalance);
        }

        [Given(@"balance now being \$(.*)")]
        [Then(@"I will receive bet response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveBetResponseOfNoErrorAndBalanceOf(decimal expectedBalance)
        {
            var response = Get<BetResponse>(SR.Response);
            response.Should().NotBeNull();
            response.ErrorCode.Should().Be((int)GameApiError.Success);
            response.Results.Length.Should().BeGreaterThan(0);

            var result = response.Results.First();
            result.ErrorCode.Should().Be((int)GameApiError.Success);
            result.Balance.Should().Be(expectedBalance);
        }

        [Then(@"I will receive bet response of error ""(.*)""")]
        public void ThenIWillReceiveBetResponseOfError(GameApiError error)
        {
            var response = Get<BetResponse>(SR.Response);

            response.Should().NotBeNull();
            response.ErrorCode.Should().Be((int)error);
            response.Balance.Should().BeNull();
        }

        [Then(@"I will receive successful response")]
        public void ThenIWillReceiveSuccessfulResponse()
        {
            var response = Get<BetResponse>(SR.Response);
            response.ErrorCode.Should().Be((int)GameApiError.Success);
        }

        [Then(@"there will be (.*) transactions in the response of error ""(.*)""")]
        public void ThenThereWillBeTransactionsInTheResponseOfError(int transactionsCount, GameApiError errorCode)
        {
            var response = Get<BetResponse>(SR.Response);
            var results = response.Results.Where(r => r.ErrorCode == (int)errorCode);
            results.Count().Should().Be(transactionsCount);
        }

        [Then(@"player wallet database record will be for \$(.*)")]
        public async Task ThenPlayerWalletDatabaseRecordWillBeForAsync(decimal expectedBalance)
        {
            var wallet = await GetPlayerWalletAsync(_serviceProvider);
            wallet.Balance.Should().Be(expectedBalance);
        }

        [Then(@"game action will be recorded in the database of type ""(.*)"" for \$(.*)")]
        public async Task ThenGameActionWillBeRecordedInTheDatabaseOfTypeForAsync(GameActionType type, decimal amount)
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

        [Then(@"all game action related database records will be created")]
        public async Task ThenAllGameActionRelatedDatabaseRecordsWillBeCreatedAsync()
        {
            var request = Get<BetRequest>(SR.Request);
            var txids = request.Transactions.Select(t => t.ExternalTransactionId).ToArray();

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var gameActions = await db.GameActions
                .AsQueryable()
                .Where(ga => txids.Contains(ga.ExternalTransactionId))
                .Select(ga => new { ga.Round.PlayerCode, ga.Id }).ToArrayAsync();

            gameActions.Length.Should().Be(txids.Length);
        }

        [Then(@"the balance of each user will be matched")]
        public void ThenTheBalanceOfEachUserWillBeMatched(Table table)
        {
            var balances = table.CreateList<PlayerData>();
            var request = Get<BetRequest>(SR.Request);
            var response = Get<BetResponse>(SR.Response);
            var betPlayers = Get<Dictionary<string, PlayerData>>(SR.Players);

            var results = from result in response.Results
                          join transaction in request.Transactions
                          on result.ExternalTransactionId equals transaction.ExternalTransactionId
                          join player in betPlayers
                          on transaction.PlayerCode equals player.Value.PlayerCode
                          select new
                          {
                              Username = player.Value.Username,
                              Balance = result.Balance
                          };

            foreach (var result in results)
            {
                var balance = balances.First(b => b.Username == result.Username);
                result.Balance.Should().Be(result.Balance);
            }
        }

        [Then(@"the round will be (.*)")]
        public Task ThenTheRoundWillBeAsync(RoundStatus status) =>
            VerifyRoundStatus(_serviceProvider, status);

        private async Task PlaceBetAsync(decimal amount, string externalGameId, string externalRoundId = null, bool validProductWallet = true)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var currency = Get<string>(SR.CurrencyCode);
            var platformType = Get<PlatformType>(SR.PlatformType);
            var config = await GetConfigurationAsync();

            externalRoundId ??= NewRoundId();
            var externalTransactionId = $"{externalRoundId}_bet";

            var request = new BetRequest
            {
                ProductWallet = validProductWallet ? GetProductWallet(config) : "0",
                SessionToken = SessionToken,
                Transactions = new[]
                {
                    new TransactionRequest
                    {
                        PlayerCode = $"{Prefix}_{playerCode}",
                        ActionType = ActionType.PlaceBet,
                        Amount = amount,
                        Currency = currency,
                        ExternalGameId = externalGameId,
                        ExternalRoundId = externalRoundId,
                        ExternalTransactionId = $"{externalRoundId}_bet",
                        Platform = platformType.ToString().ToUpper(),
                        Timestamp = DateTimeOffset.Now
                    }
                }
            };

            var response = await _client.PlaceBetAsync(request);

            Set(SR.ExternalTransactionId, externalTransactionId);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        private async Task SettleAndCancelBetAsync(decimal amount, ActionType type, string externalGameId = null)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var currency = Get<string>(SR.CurrencyCode);
            var platformType = Get<PlatformType>(SR.PlatformType);
            externalGameId ??= Get<string>(SR.ExternalGameId);

            var betRequest = Get<BetRequest>(SR.Request);

            var transaction = betRequest.Transactions.First();
            var externalRoundId = transaction.ExternalRoundId;
            var externalTransactionId = Guid.NewGuid().ToString();

            var response = await SubmitSettleAndCancelBetAsync(new[]
                {
                    GetTransactionRequest(new PlayerData
                    {
                        PlayerCode = $"{Prefix}_{playerCode}",
                        Platform = platformType,
                        ActionType = type,
                        Amount = amount,
                        CurrencyCode = currency,
                        GameId = externalGameId
                    }, externalRoundId, externalTransactionId)
                });

            Set(SR.ExternalTransactionId, externalTransactionId);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.Response, response);
        }

        private async Task<BetResponse> SubmitSettleAndCancelBetAsync(TransactionRequest[] transactions)
        {
            var config = await GetConfigurationAsync();

            var request = new BetRequest
            {
                ProductWallet = GetProductWallet(config),
                Transactions = transactions
            };

            Set(SR.Request, request);
            return await _client.SettleAndCancelBetAsync(request);
        }
        private TransactionRequest GetTransactionRequest(PlayerData playerData, string externalRoundId, string externalTransactionId)
        {
            return new TransactionRequest
            {
                PlayerCode = playerData.PlayerCode,
                ActionType = playerData.ActionType,
                Amount = playerData.Amount,
                Currency = playerData.CurrencyCode,
                ExternalGameId = playerData.GameId,
                ExternalRoundId = externalRoundId,
                ExternalTransactionId = externalTransactionId,
                Platform = playerData.Platform.ToString().ToUpper(),
                Timestamp = DateTimeOffset.Now
            };
        }

        private async Task ResentSettleBetAsync()
        {
            var request = Get<BetRequest>(SR.Request);

            var response = await _client.SettleAndCancelBetAsync(request);

            Set(SR.Response, response);
        }

        private static string NewRoundId() =>
            Guid.NewGuid().ToString();

        private Task<IMESportsConfiguration> GetConfigurationAsync() =>
                    GetSettingAsync<IMESportsConfiguration>(_serviceProvider, SettingConsts.GameProviderConfigurations.IMESports);

        private string GetProductWallet(IMESportsConfiguration config) => ((ProductWalletType)config.ProductWallet).ToString();

        private record PlayerData
        {
            public string Username { get; init; }
            public string PlayerCode { get; set; }
            public string Brand { get; init; }
            public string CurrencyCode { get; init; }
            public PlatformType Platform { get; init; }
            public decimal Balance { get; init; }
            public string GameId { get; init; }
            public decimal Amount { get; init; }
            public ActionType ActionType { get; init; }
        }
    }
}
