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
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Tokens;
using TGS.Shared.Game.Models.AsiaGaming.Enums;
using TGS.Shared.Game.Models.AsiaGaming.Incoming;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;
using TGS.Tests.TestsCommon.Extensions;
using TransactionType = TGS.Shared.Game.Models.AsiaGaming.Enums.TransactionType;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding]
    [Scope(Tag = "AsiaGaming")]
    public sealed class AsiaGamingEndpoint : BaseGameEndpoint
    {
        private const string AgentCode = "JH2";
        private static readonly string GameProviderCode = Consts.GameProvider.AsiaGaming;
        private readonly IAsiaGamingApiClient _client;
        private readonly ServiceProvider _serviceProvider;

        public AsiaGamingEndpoint(ScenarioContext context) : base(context)
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
                .AddRedis();

            services.AddNatsMessageQueue(natsConfig =>
            {
                natsConfig
                    .ConfigQueueOptions((options, sp) =>
                    {
                        var config = sp.GetRequiredService<IConfiguration>();

                        options.Url = config.GetValue<string>("NatsSetting:Urls");
                    })
                    .ProcessAllSubjects();
            });

            var isDev = configuration.GetValue<bool>("ApiUrls:IsDev");

            if (isDev)
            {
                services.AddTransient<IPlayerApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<PlayerApiClient>(sp, Hooks.OpApiClient));

                services.AddTransient<IAsiaGamingApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<AsiaGamingApiClient>(sp, Hooks.TgsApiClient));
            }
            else
            {
                services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Operator")));

                services.AddHttpClient<IAsiaGamingApiClient, AsiaGamingApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Tgs-AsiaGaming")));
            }

            _serviceProvider = services.BuildServiceProvider();
            _client = _serviceProvider.GetRequiredService<IAsiaGamingApiClient>();
        }

        [Given(@"player ""(.*)"" of brand ""(.*)"" with ""(.*)"" login via ""(.*)""")]
        public Task GivenPlayerOfBrandWithLoginVia(string username, string brandCode,
            string currencyCode, PlatformType platformType) =>
            PlayerLoginAsync(_serviceProvider, username, platformType, brandCode, currencyCode);

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

        [Given(@"player having balance of \$(.*)")]
        public Task GivenPlayerHavingBalanceOfAsync(decimal balance) =>
            SetPlayerBalanceAsync(_serviceProvider, balance);

        [Given(@"balance now being \$(.*)")]
        public async Task GivenBalanceNowBeing(decimal balance)
        {
            var wallet = await GetPlayerWalletAsync(_serviceProvider);
            wallet.Balance.Should().Be(balance);
        }

        [Given(@"player logout")]
        public Task GivenPlayerLogoutAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            return DeauthorizePlayerAsync(_serviceProvider, playerCode);
        }

        [Given(@"I place bet of \$(.*) for game ""(.*)""")]
        [When(@"I place bet of \$(.*) for game ""(.*)""")]
        public Task WhenIPlaceBetOfForGameAsync(decimal amount, string externalGameId) =>
            PlaceBetAsync(amount, externalGameId);

        [Given(@"I place bet of \$(.*) for game ""(.*)"" for the same round")]
        public Task GivenIPlaceBetOfForGameForTheSameRoundAsync(decimal amount, string externalGameId)
        {
            var betRequest = Get<PostTransferRequest>(SR.PlaceBetRequest);
            return PlaceBetAsync(amount, externalGameId, betRequest.Record.RoundId);
        }

        [Given(@"I send bet response")]
        [When(@"I send bet response")]
        public Task WhenISendBetResponse(Table table)
        {
            var records = table.CreateList<BetRecord>();
            return SendBetResponse(records.ToArray());
        }

        [When(@"I send bet response with no record")]
        public Task WhenISendBetResponseWithNoRecord() =>
            SendBetResponse(null);

        [Given(@"settle bet for win amount \$(.*)")]
        [When(@"I settle bet for win amount \$(.*)")]
        public Task WhenISettleBetForWinAmount(decimal winAmount) =>
            SettleBetAsync(winAmount);

        [When(@"I settle bet")]
        public async Task WhenISettleBet(Table table)
        {
            var records = table.CreateList<TransferRecord>();
            var betResponseRequest = Get<BetResponseRequest>(SR.Request);
            var transactionId = betResponseRequest.TransactionId;

            for (var i = 0; i < records.Count(); i++)
            {
                var record = records.ElementAt(i);
                await SettleBetAsync(record.WinAmount, betResponseRequest.Records.ElementAt(i).Amount,
                    isLockGame: true,
                    billNo: GetBillNo(transactionId, record.PlayType), finish: record.RoundFinish);
            }
        }

        [Given(@"I cancel transaction")]
        [When(@"I cancel transaction")]
        public Task WhenICancelTransaction() =>
            CancelTransactionAsync();

        [When(@"I cancel non-existent transaction for game ""(.*)""")]
        public Task WhenICancelNonExistentTransactionForGame(string externalGameId) =>
            CancelTransactionAsync(externalGameId, true);

        [When(@"I resend the same bet")]
        [When(@"I resend the same settle bet")]
        [When(@"I resend place and settle bet")]
        public async Task WhenIResendTheSameBet()
        {
            var request = Get<PostTransferRequest>(SR.Request);
            var response = await _client.PostTransferAsync(request);

            Set(SR.Response, response);
        }

        private async Task PlaceBetAsync(decimal amount, string externalGameId, string externalRoundId = null)
        {
            var platform = Get<PlatformType>(SR.PlatformType);
            var loginRecordId = GetLoginRecordId(platform);
            var gameProviderCurrencyCode = await GetGameProviderCurrencyCodeAsync();

            externalRoundId ??= NewRoundId();
            var externalTransactionId = Guid.NewGuid().ToString();
            var splitGameId = GetGameId(externalGameId);
            var gamePlatformType = splitGameId[0];
            var gameRound = splitGameId[1];
            var gameType = splitGameId[2];

            var request = new PostTransferRequest
            {
                Record = new()
                {
                    DeviceType = platform.ToString().ToUpper(),
                    Currency = gameProviderCurrencyCode,
                    SessionToken = loginRecordId.ToString(),
                    Amount = amount,
                    PlayerCode = GetAsiaGamingPlayerCode(),
                    AgentCode = AgentCode,
                    BetTime = GetTimestamp(),
                    TransactionIds = new[] { externalTransactionId },
                    RoundId = externalRoundId,
                    GamePlatformType = gamePlatformType,
                    GameRound = gameRound,
                    GameType = gameType,
                    TableCode = "va21",
                    TransactionType = TransactionType.BET,
                    TransactionCode = "BCB",
                    PlayType = gameType.IgnoreCaseEquals("ROU") ? "0" : "1"
                }
            };

            var response = await _client.PostTransferAsync(request);

            var transactionIds = Get<List<string>>(SR.ExternalTransactionId) ?? new List<string>();
            transactionIds.Add(externalTransactionId);
            Set(SR.GameProviderCurrencyCode, gameProviderCurrencyCode);
            Set(SR.ExternalTransactionId, transactionIds);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.PlaceBetRequest, request);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        private async Task SendBetResponse(BetRecord[] records)
        {
            var lockRequest = Get<PostTransferRequest>(SR.Request);
            var transactionId = lockRequest.Record.TransactionId;

            if (records != null)
            {
                foreach (var record in records)
                {
                    record.BillNo = GetBillNo(transactionId, record.PlayType);
                }
            }

            var request = new BetResponseRequest
            {
                Records = records,
                BetResponse = "OK",
                PlayerCode = lockRequest.Record.PlayerCode,
                SessionToken = lockRequest.Record.SessionToken,
                TransactionId = transactionId
            };

            var response = await _client.BetResponseAsync(request);

            Set(SR.ExternalTransactionId, new List<string> { transactionId });
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        private async Task SettleBetAsync(decimal winAmount, decimal? betAmount = null, bool isRetry = false,
            bool isLockGame = false, TransactionType? type = null, string billNo = null, string finish = null)
        {
            var transactionType = type ?? (winAmount > 0 ? TransactionType.WIN : TransactionType.LOSE);

            var platform = Get<PlatformType>(SR.PlatformType);
            var loginRecordId = GetLoginRecordId(platform);
            var gameProviderCurrencyCode = await GetGameProviderCurrencyCodeAsync();

            var betRequest = Get<PostTransferRequest>(SR.PlaceBetRequest);
            var transactionIds = isLockGame
                ? new() { betRequest.Record.TransactionId }
                : Get<List<string>>(SR.ExternalTransactionId);
            var gamePlatformType = betRequest.Record.GamePlatformType;
            var gameRound = betRequest.Record.GameRound;
            var gameType = betRequest.Record.GameType;

            var settleRequest = new PostTransferRequest
            {
                Record = new()
                {
                    DeviceType = platform.ToString().ToUpper(),
                    Currency = gameProviderCurrencyCode,
                    SessionToken = loginRecordId.ToString(),
                    WinAmount = winAmount,
                    BetAmount = betAmount ?? betRequest.Record.Amount,
                    PlayerCode = GetAsiaGamingPlayerCode(),
                    AgentCode = AgentCode,
                    BetTime = betRequest.Record.BetTime,
                    SettleTime = GetTimestamp(),
                    TransactionIds = transactionIds.ToArray(),
                    BillNo = billNo,
                    RoundId = betRequest.Record.RoundId,
                    GamePlatformType = gamePlatformType,
                    GameRound = gameRound,
                    GameType = gameType,
                    TableCode = "va21",
                    TransactionType = transactionType,
                    GameResult = "P;HK;DQ;HJ:B;H5;CK;DA",
                    TransactionCode = "BCB",
                    RoundFinish = finish ?? "true",
                    PlayType = "1"
                }
            };

            var response = await _client.PostTransferAsync(settleRequest);

            Set(SR.ExternalTransactionId, transactionIds.Select(t => $"{t}_{transactionType}").ToList());
            Set(SR.Request, settleRequest);
            Set(SR.Response, response);
        }

        private async Task CancelTransactionAsync(string gameId = null, bool newRound = false)
        {
            var platform = Get<PlatformType>(SR.PlatformType);
            var loginRecordId = GetLoginRecordId(platform);
            var gameProviderCurrencyCode = await GetGameProviderCurrencyCodeAsync();

            string transactionId, roundId;
            var gamePlatformType = string.Empty;
            var gameRound = string.Empty;
            var gameType = string.Empty;

            if (newRound)
            {
                roundId = NewRoundId();
                transactionId = roundId;
            }
            else
            {
                var betRequest = Get<PostTransferRequest>(SR.PlaceBetRequest);
                transactionId = betRequest.Record.TransactionId;
                roundId = betRequest.Record.RoundId;
                gamePlatformType = betRequest.Record.GamePlatformType;
                gameRound = betRequest.Record.GameRound;
                gameType = betRequest.Record.GameType;
            }

            if (gameId.IsNotNullOrEmpty())
            {
                var splitGameId = GetGameId(gameId);
                gamePlatformType = splitGameId[0];
                gameRound = splitGameId[1];
                gameType = splitGameId[2];
            }

            var cancelRequest = new PostTransferRequest
            {
                Record = new()
                {
                    DeviceType = platform.ToString().ToUpper(),
                    Currency = gameProviderCurrencyCode,
                    SessionToken = loginRecordId.ToString(),
                    PlayerCode = GetAsiaGamingPlayerCode(),
                    AgentCode = AgentCode,
                    BetTime = GetTimestamp(),
                    TransactionIds = new[] { transactionId },
                    RoundId = roundId,
                    GamePlatformType = gamePlatformType,
                    GameRound = gameRound,
                    GameType = gameType,
                    TableCode = "va21",
                    TransactionType = TransactionType.REFUND,
                    TransactionCode = "BCB"
                }
            };

            var response = await _client.PostTransferAsync(cancelRequest);

            Set(SR.ExternalTransactionId, new List<string> { $"{transactionId}_{TransactionType.REFUND}" });
            Set(SR.Request, cancelRequest);
            Set(SR.Response, response);
        }

        private Guid GetLoginRecordId(PlatformType platform)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var key = $"{SR.PlayerTokenData}:{playerCode}:{platform}";
            var tokenData = Get<PlayerTokenData>(key);
            return tokenData?.LoginRecordId ?? Guid.Empty;
        }

        [Then(@"I will receive bet response of (.*) and balance of \$(.*)")]
        public void ThenIWillReceiveBetResponseOfNoErrorAndBalanceOf(GameApiError responseCode, decimal expectedBalance)
        {
            var response = Get<TransferResponse>(SR.Response);

            response.Should().NotBeNull();
            response.ResponseCode.Should().Be(responseCode);
            response.Balance.Should().Be(expectedBalance);
        }

        [Then(@"player wallet database record will be for \$(.*)")]
        public async Task ThenPlayerWalletDatabaseRecordWillBeForAsync(decimal expectedBalance)
        {
            var wallet = await GetPlayerWalletAsync(_serviceProvider);
            wallet.Balance.Should().Be(expectedBalance);
        }

        [Then(@"game action id contains (.*) will be recorded in the database of type ""(.*)"" for \$(.*)")]
        public async Task ThenGameActionIdContainsWillBeRecordedInTheDatabaseOfTypeFor(string playType,
            GameActionType type, decimal amount)
        {
            var gas = await GetGameActionsByRoundId();
            var placeGa = gas.First(ga => ga.ExternalTransactionId.IgnoreCaseContains(playType)
                && ga.Type == GameActionType.PlaceBet);
            var ga = gas.First(ga => ga.ReferencedGameActionId == placeGa.Id);
            ga.Type.Should().Be(type);
            ga.Amount.Should().Be(amount);
        }

        [Then(@"(.*) game actions will be recorded in the database of type ""(.*)"" for \$(.*)")]
        public async Task ThenGameActionsWillBeRecordedInTheDatabaseOfTypeForAsync(int count, GameActionType type,
            decimal amount)
        {
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var externalTransactionIds = Get<List<string>>(SR.ExternalTransactionId);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var gas = await (
                from act in db.GameActions.AsQueryable()
                join r in db.Rounds on act.RoundId equals r.Id
                where r.ExternalRoundId == externalRoundId &&
                    externalTransactionIds.Contains(act.ExternalTransactionId)
                select act).AsNoTracking().ToArrayAsync();

            gas.Should().NotBeNull();
            gas.Length.Should().Be(count);
            foreach (var ga in gas)
            {
                ga.Type.Should().Be(type);
                ga.Amount.Should().Be(amount);
            }
        }

        [Then(@"game action will be recorded in the database of type ""(.*)"" for \$(.*)")]
        public async Task ThenGameActionWillBeRecordedInTheDatabaseOfTypeForAsync(GameActionType type, decimal amount)
        {
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var externalTransactionIds = Get<List<string>>(SR.ExternalTransactionId);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var ga = await (
                from act in db.GameActions.AsQueryable()
                join r in db.Rounds on act.RoundId equals r.Id
                where r.ExternalRoundId == externalRoundId &&
                    externalTransactionIds.Contains(act.ExternalTransactionId)
                select act).AsNoTracking().FirstAsync();

            Set(SR.GameAction, ga);

            ga.Should().NotBeNull();
            ga.Type.Should().Be(type);
            ga.Amount.Should().Be(amount);
        }

        [Then(@"round will have record of type ""(.*)"" in the database for \$(.*)")]
        public async Task ThenRoundWillHaveRecordOfTypeInTheDatabaseFor(GameActionType type, decimal amount)
        {
            var gas = await GetGameActionsByRoundId();

            Set(SR.GameAction, gas);

            gas.Should().NotBeNull();
            var ga = gas.First(ga => ga.Type == type);
            ga.Should().NotBeNull();
            ga.Amount.Should().Be(amount);
        }

        [Then(@"round will have (.*) record of type ""(.*)"" in the database")]
        public async Task ThenRoundWillHaveRecordOfTypeInTheDatabase(int count, GameActionType type)
        {
            var gas = await GetGameActionsByRoundId();

            gas.Should().NotBeNull();

            var gatype = gas.Where(ga => ga.Type == type);
            gatype.Count().Should().Be(count);
        }

        [Then(@"the round will be closed")]
        public Task ThenTheRoundWillBeClosedAsync() =>
            VerifyRoundStatus(_serviceProvider, RoundStatus.Closed);

        private Task<GameAction[]> GetGameActionsByRoundId()
        {
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var db = GetDb<ITgsRepository>(_serviceProvider);
            return (
                from act in db.GameActions.AsQueryable()
                join r in db.Rounds on act.RoundId equals r.Id
                where r.ExternalRoundId == externalRoundId
                select act).AsNoTracking().ToArrayAsync();
        }

        private static string NewRoundId() => Guid.NewGuid().ToString();

        private string GetTimestamp() => DateTimeOffset.UtcNow.AddHours(-4).ToString("MM/dd/yyyy HH:mm:sss");

        private string GetAsiaGamingPlayerCode()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            return $"{AgentCode}{playerCode}";
        }

        private Task<string> GetGameProviderCurrencyCodeAsync()
        {
            var currencyCode = Get<string>(SR.CurrencyCode);
            var db = GetDb<ITgsRepository>(_serviceProvider);
            return db.GameProviderCurrencies
                .AsQueryable()
                .Where(c => c.GameProvider.Code == GameProviderCode && c.CurrencyCode == currencyCode)
                .Select(c => c.GameProviderCurrencyCode).FirstOrDefaultAsync();
        }

        private string[] GetGameId(string gameId)
            => gameId.Split('_');

        private string GetBillNo(string transactionId, string playType)
            => $"{transactionId}_{playType}";
    }
}