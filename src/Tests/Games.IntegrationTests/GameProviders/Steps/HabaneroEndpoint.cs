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
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Tokens;
using TGS.Shared.Game.Models.Habanero.Enums;
using TGS.Shared.Game.Models.Habanero.Incoming;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;
using static TGS.Core.Models.Enums.GameActionType;
using Status = TGS.Core.Models.Enums.Status;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding]
    [Scope(Tag = "Habanero")]
    public sealed class HabaneroEndpoint : BaseGameEndpoint
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Habanero;
        private static readonly string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK";

        private readonly Authorization _authorization = new()
        {
            HabaneroBrandId = "a5df0ada-7b6a-eb11-9889-00155db5435a",
            ClientId = "habanero",
            SecretKey = "NzrgFf4Bb5KuXy9",
            MachineName = "TESTSERVER",
            Culture = "en"
        };

        private readonly ServiceProvider _serviceProvider;

        public HabaneroEndpoint(ScenarioContext context)
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
                .AddRedis();

            var isDev = configuration.GetValue<bool>("ApiUrls:IsDev");

            if (isDev)
            {
                services.AddTransient<IPlayerApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<PlayerApiClient>(sp, Hooks.OpApiClient));

                services.AddTransient<IHabaneroApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<HabaneroApiClient>(sp, Hooks.TgsApiClient));
            }
            else
            {
                services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Operator")));

                services.AddHttpClient<IHabaneroApiClient, HabaneroApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Tgs-Habanero")));
            }

            _serviceProvider = services.BuildServiceProvider();
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

        [Given(@"player logout")]
        public Task GivenPlayerLogoutAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            return DeauthorizePlayerAsync(_serviceProvider, playerCode);
        }

        [Given(@"disable the player")]
        public Task GivenDisableThePlayer()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            return ChangePlayerStatusAsync(_serviceProvider, playerCode, PlayerStatus.Disabled);
        }

        [Given(@"player having balance of \$(.*)")]
        public Task GivenPlayerHavingBalanceOfAsync(decimal balance) =>
            SetPlayerBalanceAsync(_serviceProvider, balance);

        [Given(@"game provider has status set to ""(.*)""")]
        public Task GivenGameProviderHasStatusSetToAsync(Status status) =>
            ChangeGameProviderStatusAsync(_serviceProvider, GameProviderCode, status);

        [Given(@"brand has status set to ""(.*)""")]
        public Task GivenBrandHasStatusSetToAsync(Status status) =>
            ChangeBrandStatusAsync(_serviceProvider, status);

        [Given(@"balance now being \$(.*)")]
        public async Task GivenBalanceNowBeing(decimal balance)
        {
            var wallet = await GetPlayerWalletAsync(_serviceProvider);
            wallet.Balance.Should().Be(balance);
        }

        [Given(@"I placed bet for \$(.*) and settle it for \$(.*) for game ""(.*)""")]
        [When(@"I place bet for \$(.*) and settle it for \$(.*) for game ""(.*)""")]
        public Task GivenIPlacedBetForAndSettleItForForGame(decimal betAmount, decimal winAmount,
            string externalGameId) =>
            PlaceBetAndSettleAsync(betAmount, winAmount, externalGameId);

        [Given(@"I place bet of \$(.*) for game ""(.*)""")]
        [When(@"I place bet of \$(.*) for game ""(.*)""")]
        public Task WhenIPlaceBetOfForGameAsync(decimal amount, string externalGameId) =>
            PlaceBetAsync(amount, externalGameId);

        [When(@"I call authorize endpoint with username=""(.*)"" and passkey=""(.*)"" and brandid=""(.*)""")]
        public async Task WhenICallDetailsEndpointWithUsernameAndPasswordAndPlayerAuthToken(string clientId,
            string secretKey, string habaneroBrandId)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);
            var client = _serviceProvider.GetRequiredService<IHabaneroApiClient>();

            var response = await client.AuthorizePlayerAsync(new()
            {
                Type = "playerdetailrequest",
                SentOn = DateTimeOffset.Now,
                Game = new()
                {
                    ExternalRtpId = "af834712-47b4-4063-b63b-a826979a01e1", ExternalId = "AcesandEights1Hand"
                },
                Authorization = new()
                {
                    ClientId = clientId,
                    SecretKey = secretKey,
                    HabaneroBrandId = habaneroBrandId,
                    MachineName = "TESTSERVER"
                },
                Detail = new() {Token = encryptedToken, GameLaunch = true}
            });

            Set(SR.Response, response);
        }

        [Given(@"I set non-existent game action")]
        public void GivenISetNonExistentGameAction()
        {
            Set(SR.ExternalTransactionId, Guid.NewGuid().ToString());
            Set(SR.ExternalRoundId, NewRoundId());
        }

        [Given(@"settle bet for \$(.*)")]
        [When(@"I settle bet for \$(.*)")]
        public Task WhenISettleBetFor(decimal winAmount) =>
            SettleBetAsync(winAmount);

        [When(@"I got a free bet for \$(.*)")]
        public Task WhenIGotAFreeBetFor(decimal amount) =>
            SettleBetAsync(amount, type: FreeBet);

        [When(@"I retry settle bet for \$(.*)")]
        public Task WhenIRetrySettleBetFor(decimal winAmount) =>
            SettleBetAsync(winAmount, true);

        [When(@"I resend the same bet")]
        [When(@"I resend the same settle bet")]
        [When(@"I resend place and settle bet")]
        public async Task WhenIResendTheSameBet()
        {
            var request = Get<FundTransferRequest>(SR.Request);

            var client = _serviceProvider.GetRequiredService<IHabaneroApiClient>();
            var response = await client.FundTransferAsync(request);

            Set(SR.Response, response);
        }

        [Given(@"I cancel transaction")]
        [When(@"I cancel transaction")]
        public Task WhenICancelTransaction() =>
            CancelTransactionAsync();

        [When(@"I cancel non-existent transaction for game ""(.*)""")]
        public Task WhenICancelNonExistentTransactionForGame(string externalGameId) =>
            CancelNonExistentTransactionForGameAsync(externalGameId);

        [When(@"I place bet for \$(.*) and settle invalid reference Id for game ""(.*)""")]
        public Task WhenIPlaceBetForAndSettleInvalidReferenceIdForGame(decimal betAmount, string externalGameId) =>
            PlaceBetAndSettleWithInvalidReferenceIdAsync(betAmount, externalGameId);

        [When(@"I query bet existence")]
        public async Task WhenIQueryBetExistence()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var transactionId = Get<string>(SR.ExternalTransactionId);
            var roundId = Get<int>(SR.ExternalRoundId);

            var client = _serviceProvider.GetRequiredService<IHabaneroApiClient>();
            var response = await client.QueryGameActionAsync(new()
            {
                Type = "queryrequest",
                Authorization = _authorization,
                Detail = new()
                {
                    PlayerCode = playerCode,
                    Token = encryptedToken,
                    TransactionId = transactionId,
                    RoundId = DateTimeOffset.UtcNow.ToString(),
                    ClientRoundId = roundId
                }
            });

            Set(SR.Response, response);
        }

        [When(@"I query bet existence without token")]
        public async Task WhenIQueryBetExistenceWithoutToken()
        {
            var playerCode = Get<string>(SR.PlayerCode);

            var transactionId = Get<string>(SR.ExternalTransactionId);
            var roundId = Get<int>(SR.ExternalRoundId);

            var client = _serviceProvider.GetRequiredService<IHabaneroApiClient>();
            var response = await client.QueryGameActionAsync(new()
            {
                Type = "queryrequest",
                Authorization = _authorization,
                Detail = new()
                {
                    PlayerCode = playerCode,
                    Token = string.Empty,
                    TransactionId = transactionId,
                    RoundId = Guid.NewGuid().ToString(),
                    ClientRoundId = roundId
                }
            });

            Set(SR.Response, response);
        }

        [Then(@"I will get authorize response and player balance will be \$(.*)")]
        public async Task ThenIWillGetAuthorizeResponseAndPlayerBalanceWillBe(decimal balance)
        {
            var wallet = await GetPlayerWalletAsync(_serviceProvider);

            var response = Get<AuthorizePlayerResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Result.Status.Success.Should().BeTrue();
            response.Result.Status.HasAuthenticationError.Should().BeFalse();
            response.Result.Balance.Should().Be(balance);
            response.Result.CurrencyCode.Should().Be(wallet.CurrencyCode);
            response.Result.PlayerCode.Should().Be(wallet.PlayerCode);
            response.Result.PlayerUsername.Should().NotBeNullOrEmpty();
        }

        [Then(@"I will get error authorize response and no player balance")]
        public void ThenIWillGetErrorDetailsResponseAndPlayerBalanceWillBe()
        {
            var response = Get<AuthorizePlayerResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Result.Status.Success.Should().BeFalse();
            response.Result.Status.HasAuthenticationError.Should().BeNull();
            response.Result.Balance.Should().BeNull();
            response.Result.CurrencyCode.Should().BeNullOrEmpty();
            response.Result.PlayerCode.Should().BeNullOrEmpty();
            response.Result.PlayerUsername.Should().BeNullOrEmpty();
        }

        [Then(@"I will receive bet response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveBetResponseOfNoErrorAndBalanceOf(decimal expectedBalance)
        {
            var request = Get<FundTransferRequest>(SR.Request);

            var isBetAndSettle = request.Detail.FundTransfer.IsBetAndSettle;
            var gameProviderCurrencyCode = Get<string>(SR.GameProviderCurrencyCode);
            var response = Get<FundTransferResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Result.Status.Success.Should().BeTrue();

            if (isBetAndSettle)
            {
                response.Result.Status.DebitSucceeded.Should().BeTrue();
                response.Result.Status.CreditSucceeded.Should().BeTrue();
            }
            else
            {
                response.Result.Status.DebitSucceeded.Should().BeNull();
                response.Result.Status.CreditSucceeded.Should().BeNull();
            }

            response.Result.Balance.Should().Be(expectedBalance);
            response.Result.CurrencyCode.Should().Be(gameProviderCurrencyCode);
        }

        [Then(@"I will receive bet response with authorization error and no balance")]
        public void ThenIWillReceiveBetResponseWithAuthorizationErrorAndNoBalance()
        {
            var response = Get<FundTransferResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Result.Status.Success.Should().BeFalse();
            response.Result.Status.HasAuthenticationError.Should().BeTrue();
            response.Result.Balance.Should().BeNull();
            response.Result.CurrencyCode.Should().BeNullOrEmpty();
        }

        [Then(@"I will receive bet response no funds error and balance of \$(.*)")]
        public void ThenIWillReceiveBetResponseNoFundsErrorAndBalanceOf(decimal expectedBalance)
        {
            var gameProviderCurrencyCode = Get<string>(SR.GameProviderCurrencyCode);
            var response = Get<FundTransferResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Result.Status.Success.Should().BeFalse();
            response.Result.Status.HasInsufficientFunds.Should().BeTrue();
            response.Result.Balance.Should().Be(expectedBalance);
            response.Result.CurrencyCode.Should().Be(gameProviderCurrencyCode);
        }

        [Then(@"I will receive bet response debit/credit error and no balance")]
        public void ThenIWillReceiveBetResponseDebitCreditErrorAndNoBalance()
        {
            var placeBetRequest = Get<FundTransferRequest>(SR.PlaceBetRequest);
            var isBetAndSettle = placeBetRequest.Detail.FundTransfer.IsBetAndSettle;

            var response = Get<FundTransferResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Result.Status.Success.Should().BeFalse();
            response.Result.Status.HasAuthenticationError.Should().BeNull();

            if (isBetAndSettle)
            {
                response.Result.Status.DebitSucceeded.Should().BeFalse();
                response.Result.Status.CreditSucceeded.Should().BeFalse();
            }
            else
            {
                response.Result.Status.DebitSucceeded.Should().BeNull();
                response.Result.Status.CreditSucceeded.Should().BeNull();
            }

            response.Result.Balance.Should().BeNull();
            response.Result.CurrencyCode.Should().BeNullOrEmpty();
        }

        [Then(@"I will receive bet response debit/credit error and balance of \$(.*)")]
        public void ThenIWillReceiveBetResponseDebitCreditErrorAndBalanceOf(decimal expectedBalance)
        {
            var gameProviderCurrencyCode = Get<string>(SR.GameProviderCurrencyCode);
            var placeBetRequest = Get<FundTransferRequest>(SR.PlaceBetRequest);
            var isBetAndSettle = placeBetRequest.Detail.FundTransfer.IsBetAndSettle;

            var response = Get<FundTransferResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Result.Status.Success.Should().BeFalse();
            response.Result.Status.HasAuthenticationError.Should().BeNull();

            if (isBetAndSettle)
            {
                response.Result.Status.DebitSucceeded.Should().BeFalse();
                response.Result.Status.CreditSucceeded.Should().BeFalse();
            }
            else
            {
                response.Result.Status.DebitSucceeded.Should().BeNull();
                response.Result.Status.CreditSucceeded.Should().BeNull();
            }

            response.Result.Balance.Should().Be(expectedBalance);
            response.Result.CurrencyCode.Should().Be(gameProviderCurrencyCode);
        }

        [Then(@"I will receive ""(.*)"" cancel response and balance of \$(.*)")]
        public void ThenIWillReceiveCancelResponseAndBalanceOf(CancelStatus status, decimal expectedBalance)
        {
            var gameProviderCurrencyCode = Get<string>(SR.GameProviderCurrencyCode);
            var response = Get<FundTransferResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Result.Status.Success.Should().BeTrue();
            response.Result.Status.CancelStatus.Should().Be(status);
            response.Result.Balance.Should().Be(expectedBalance);
            response.Result.CurrencyCode.Should().Be(gameProviderCurrencyCode);
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
            var externalRoundId = Get<int>(SR.ExternalRoundId);
            var externalTransactionId = Get<string>(SR.ExternalTransactionId);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var ga = await (
                from act in db.GameActions.AsQueryable()
                join r in db.Rounds on act.RoundId equals r.Id
                where r.ExternalRoundId == externalRoundId.ToString() &&
                    act.ExternalTransactionId == externalTransactionId
                select act).AsNoTracking().FirstAsync();

            Set(SR.GameAction, ga);

            ga.Should().NotBeNull();
            ga.Type.Should().Be(type);
            ga.Amount.Should().Be(amount);
        }

        [Then(@"#(.*) game action will be recorded in the database of type ""(.*)"" for \$(.*)")]
        public async Task ThenGameActionWillBeRecordedInTheDatabaseOfTypeForAmount(int transactionIndex,
            GameActionType type, decimal amount)
        {
            var fundTransferRequest = Get<FundTransferRequest>(SR.Request);
            var externalTransactionId = GetExternalTransactionId(fundTransferRequest, transactionIndex);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var gameAction = await db.GameActions
                .AsQueryable()
                .FirstAsync(ga => ga.ExternalTransactionId == externalTransactionId);

            gameAction.Type.Should().Be(type, " The type of the game action should be as expected ");
            gameAction.Amount.Should().Be(amount, "The amount of the transaction should be as expected");
        }

        [Then(@"\#(.*) game action will NOT be recorded in the database")]
        public async Task ThenGameActionWillNOTBeRecordedInTheDatabase(int transactionIndex)
        {
            var fundTransferRequest = Get<FundTransferRequest>(SR.Request);
            var externalTransactionId = GetExternalTransactionId(fundTransferRequest, transactionIndex);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var gameAction = await db.GameActions
                .AsQueryable()
                .FirstOrDefaultAsync(ga => ga.ExternalTransactionId == externalTransactionId);

            gameAction.Should().BeNull();
        }

        [Then(@"I get ""(.*)"" query result")]
        public void ThenIGetTrueQueryResult(bool expectedResult)
        {
            var response = Get<QueryResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Result.Status.Success.Should().Be(expectedResult);
        }

        #region After Scenario

        [After]
        public Task AfterChangeGameProviderStatusScenarioAsync() =>
            ChangeGameProviderStatusAsync(_serviceProvider, GameProviderCode, Status.Active, true);

        [After]
        public Task AfterChangeGameProviderConfigurationStatusScenarioAsync() =>
            ChangeGameProviderConfigurationStatusAsync(_serviceProvider, GameProviderCode, Status.Active, true);

        [After]
        public Task AfterChangeGameStatusScenarioAsync() =>
            ChangeGameStatusAsync(_serviceProvider, GameProviderCode, Get<string>(SR.ExternalGameId),
                GameStatus.Active, true);

        [After]
        public Task AfterChangeBrandStatusScenarioAsync() =>
            ChangeBrandStatusAsync(_serviceProvider, Status.Active, true);

        [After]
        public Task AfterChangePlayerStatusScenarioAsync()
        {
            if (!HasTag("DisablePlayer"))
            {
                return Task.CompletedTask;
            }

            var playerCode = Get<string>(SR.PlayerCode);
            return ChangePlayerStatusAsync(_serviceProvider, playerCode, PlayerStatus.Enabled);
        }

        #endregion After Scenario

        #region Private Method

        private static string GetExternalTransactionId(FundTransferRequest fundTransferRequest,
            int? transactionIndex = null) =>
            fundTransferRequest.Detail.IsCancel
                ? fundTransferRequest.Detail.FundTransfer.CancelTransaction.TransactionId
                : fundTransferRequest.Detail.FundTransfer.Transactions[(transactionIndex ?? 1) - 1].TransactionId;

        private static int NewRoundId() => (int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        private Task<string> GetEncryptedTokenAsync(string playerCode, PlatformType platform)
        {
            var key = $"{SR.PlayerTokenData}:{playerCode}:{platform}";
            var tokenData = Get<PlayerTokenData>(key);
            var loginRecordId = tokenData?.LoginRecordId ?? Guid.Empty;

            var tokenProvider = _serviceProvider.GetRequiredService<ITokenProvider>();
            return tokenProvider.EncryptAsync(loginRecordId);
        }

        private async Task PlaceBetAsync(decimal amount, string externalGameId, int? externalRoundId = null)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var currencyCode = Get<string>(SR.CurrencyCode);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var gameProviderCurrencyCode = await db.GameProviderCurrencies
                .AsQueryable()
                .Where(c => c.GameProvider.Code == GameProviderCode && c.CurrencyCode == currencyCode)
                .Select(c => c.GameProviderCurrencyCode).FirstOrDefaultAsync();

            externalRoundId ??= NewRoundId();
            var externalTransactionId = Guid.NewGuid().ToString();
            var client = _serviceProvider.GetRequiredService<IHabaneroApiClient>();

            var now = DateTimeOffset.Now;

            var request = new FundTransferRequest
            {
                Type = "fundtransferrequest",
                SentOn = now,
                Authorization = _authorization,
                Game = new() {ExternalId = externalGameId},
                Detail = new()
                {
                    RoundId = externalTransactionId,
                    ClientRoundId = externalRoundId.Value,
                    IsCancel = false,
                    IsRetry = false,
                    RetryCount = 0,
                    IsSettlementRetry = false,
                    FundTransfer = new()
                    {
                        IsBetAndSettle = false,
                        Transactions = new Shared.Game.Models.Habanero.Incoming.Transaction[]
                        {
                            new()
                            {
                                GameState = GameState.NewGame,
                                TransactionId = externalTransactionId,
                                CurrencyCode = gameProviderCurrencyCode,
                                Amount = -amount,
                                TransactionReferenceId = externalTransactionId,
                                Timestamp = now.ToString(TimeFormat)
                            }
                        }
                    },
                    Token = encryptedToken,
                    PlayerCode = playerCode
                }
            };

            var response = await client.FundTransferAsync(request);

            Set(SR.GameProviderCurrencyCode, gameProviderCurrencyCode);
            Set(SR.ExternalTransactionId, externalTransactionId);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.PlaceBetRequest, request);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        private async Task SettleBetAsync(decimal winAmount, bool isRetry = false, GameActionType? type = null)
        {
            var transactionId = Guid.NewGuid().ToString();
            var now = DateTimeOffset.Now;
            var gameType = type ?? (winAmount > 0 ? WinBet : LoseBet);
            var isFreeBet = gameType == FreeBet;

            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var betRequest = Get<FundTransferRequest>(SR.PlaceBetRequest);
            var settleRequest = new FundTransferRequest
            {
                Type = "fundtransferrequest",
                SentOn = now,
                Authorization = _authorization,
                Game = new() {ExternalId = betRequest.Game.ExternalId},
                Detail = new()
                {
                    RoundId = betRequest.Detail.RoundId,
                    ClientRoundId = betRequest.Detail.ClientRoundId,
                    IsCancel = false,
                    IsRetry = isRetry,
                    RetryCount = 0,
                    IsSettlementRetry = isRetry,
                    FundTransfer = new()
                    {
                        IsBetAndSettle = false,
                        Transactions = new Shared.Game.Models.Habanero.Incoming.Transaction[]
                        {
                            new()
                            {
                                GameState = isFreeBet ? GameState.Ignore : GameState.Completing,
                                IsFreeSpin = isFreeBet,
                                TransactionId = transactionId,
                                CurrencyCode = Get<string>(SR.GameProviderCurrencyCode),
                                Amount = winAmount,
                                TransactionReferenceId = GetExternalTransactionId(betRequest),
                                Timestamp = now.ToString(TimeFormat)
                            }
                        }
                    },
                    Token = encryptedToken,
                    PlayerCode = playerCode
                }
            };

            var client = _serviceProvider.GetRequiredService<IHabaneroApiClient>();
            var response = await client.FundTransferAsync(settleRequest);

            Set(SR.ExternalTransactionId, transactionId);
            Set(SR.Request, settleRequest);
            Set(SR.Response, response);
        }

        private async Task PlaceBetAndSettleAsync(decimal betAmount, decimal winAmount, string externalGameId)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var currencyCode = Get<string>(SR.CurrencyCode);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var gameProviderCurrencyCode = await db.GameProviderCurrencies
                .AsQueryable()
                .Where(c => c.GameProvider.Code == GameProviderCode && c.CurrencyCode == currencyCode)
                .Select(c => c.GameProviderCurrencyCode).FirstOrDefaultAsync();

            var externalRoundId = NewRoundId();
            var betExternalTransactionId = Guid.NewGuid().ToString();
            var client = _serviceProvider.GetRequiredService<IHabaneroApiClient>();

            var now = DateTimeOffset.Now;

            var request = new FundTransferRequest
            {
                Type = "fundtransferrequest",
                SentOn = now,
                Authorization = _authorization,
                Game = new() {ExternalId = externalGameId},
                Detail = new()
                {
                    RoundId = betExternalTransactionId,
                    ClientRoundId = externalRoundId,
                    IsCancel = false,
                    IsRetry = false,
                    RetryCount = 0,
                    IsSettlementRetry = false,
                    FundTransfer = new()
                    {
                        IsBetAndSettle = true,
                        Transactions = new Shared.Game.Models.Habanero.Incoming.Transaction[]
                        {
                            new()
                            {
                                GameState = GameState.NewGame,
                                TransactionId = betExternalTransactionId,
                                CurrencyCode = gameProviderCurrencyCode,
                                Amount = -betAmount,
                                TransactionReferenceId = betExternalTransactionId,
                                Timestamp = now.ToString(TimeFormat)
                            },
                            new()
                            {
                                GameState = GameState.Completing,
                                TransactionId = Guid.NewGuid().ToString(),
                                CurrencyCode = gameProviderCurrencyCode,
                                Amount = winAmount,
                                TransactionReferenceId = betExternalTransactionId,
                                Timestamp = now.ToString(TimeFormat)
                            }
                        }
                    },
                    Token = encryptedToken,
                    PlayerCode = playerCode
                }
            };

            var response = await client.FundTransferAsync(request);

            Set(SR.GameProviderCurrencyCode, gameProviderCurrencyCode);
            Set(SR.ExternalTransactionId, betExternalTransactionId);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.PlaceBetRequest, request);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        private async Task CancelTransactionAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var betRequest = Get<FundTransferRequest>(SR.PlaceBetRequest);
            var betTransaction = betRequest.Detail.FundTransfer.Transactions[0];

            var transactionId = Guid.NewGuid().ToString();
            var cancelRequest = new FundTransferRequest
            {
                Type = "fundtransferrequest",
                Authorization = _authorization,
                Game = new() {ExternalId = betRequest.Game.ExternalId},
                Detail = new()
                {
                    RoundId = betRequest.Detail.RoundId,
                    ClientRoundId = betRequest.Detail.ClientRoundId,
                    IsCancel = true,
                    IsRetry = true,
                    RetryCount = 0,
                    IsSettlementRetry = false,
                    FundTransfer = new()
                    {
                        IsBetAndSettle = false,
                        CancelTransaction = new()
                        {
                            CurrencyCode = betTransaction.CurrencyCode,
                            Amount = betTransaction.Amount,
                            Timestamp = betTransaction.Timestamp,
                            GameState = GameState.Completing,
                            TransactionId = transactionId,
                            TransactionReferenceId = betTransaction.TransactionId
                        }
                    },
                    Token = encryptedToken,
                    PlayerCode = playerCode
                }
            };

            var client = _serviceProvider.GetRequiredService<IHabaneroApiClient>();
            var response = await client.FundTransferAsync(cancelRequest);

            Set(SR.ExternalTransactionId, transactionId);
            Set(SR.Request, cancelRequest);
            Set(SR.Response, response);
        }

        private async Task CancelNonExistentTransactionForGameAsync(string externalGameId)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var currencyCode = Get<string>(SR.CurrencyCode);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var gameProviderCurrencyCode = await db.GameProviderCurrencies
                .AsQueryable()
                .Where(c => c.GameProvider.Code == GameProviderCode && c.CurrencyCode == currencyCode)
                .Select(c => c.GameProviderCurrencyCode).FirstOrDefaultAsync();

            var externalRoundId = NewRoundId();
            var externalTransactionId = Guid.NewGuid().ToString();

            var now = DateTimeOffset.Now;
            var cancelRequest = new FundTransferRequest
            {
                Type = "fundtransferrequest",
                Authorization = _authorization,
                Game = new() {ExternalId = externalGameId},
                Detail = new()
                {
                    RoundId = externalTransactionId,
                    ClientRoundId = externalRoundId,
                    IsCancel = true,
                    IsRetry = true,
                    RetryCount = 0,
                    IsSettlementRetry = false,
                    FundTransfer = new()
                    {
                        IsBetAndSettle = false,
                        CancelTransaction = new()
                        {
                            CurrencyCode = gameProviderCurrencyCode,
                            Amount = 10,
                            Timestamp = now.ToString(TimeFormat),
                            GameState = GameState.Completing,
                            TransactionId = externalTransactionId,
                            TransactionReferenceId = externalTransactionId
                        }
                    },
                    Token = encryptedToken,
                    PlayerCode = playerCode
                }
            };

            var client = _serviceProvider.GetRequiredService<IHabaneroApiClient>();
            var response = await client.FundTransferAsync(cancelRequest);

            Set(SR.GameProviderCurrencyCode, gameProviderCurrencyCode);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.ExternalTransactionId, externalTransactionId);
            Set(SR.Request, cancelRequest);
            Set(SR.Response, response);
        }

        private async Task PlaceBetAndSettleWithInvalidReferenceIdAsync(decimal betAmount, string externalGameId)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var currencyCode = Get<string>(SR.CurrencyCode);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var gameProviderCurrencyCode = await db.GameProviderCurrencies
                .AsQueryable()
                .Where(c => c.GameProvider.Code == GameProviderCode && c.CurrencyCode == currencyCode)
                .Select(c => c.GameProviderCurrencyCode).FirstOrDefaultAsync();

            var now = DateTimeOffset.Now;
            var externalRoundId = NewRoundId();
            var externalTransactionId = Guid.NewGuid().ToString();

            var request = new FundTransferRequest
            {
                Type = "fundtransferrequest",
                Authorization = _authorization,
                Game = new() {ExternalId = externalGameId},
                Detail = new()
                {
                    RoundId = externalTransactionId,
                    ClientRoundId = externalRoundId,
                    IsCancel = false,
                    IsRetry = false,
                    RetryCount = 0,
                    IsSettlementRetry = false,
                    FundTransfer = new()
                    {
                        IsBetAndSettle = true,
                        Transactions = new Shared.Game.Models.Habanero.Incoming.Transaction[]
                        {
                            new()
                            {
                                CurrencyCode = gameProviderCurrencyCode,
                                Amount = -betAmount,
                                Timestamp = now.ToString(TimeFormat),
                                GameState = GameState.NewGame,
                                TransactionId = externalTransactionId,
                                TransactionReferenceId = externalTransactionId
                            },
                            new()
                            {
                                CurrencyCode = gameProviderCurrencyCode,
                                Amount = 100,
                                Timestamp = now.ToString(TimeFormat),
                                GameState = GameState.Completing,
                                TransactionId = Guid.NewGuid().ToString(),
                                TransactionReferenceId = Guid.NewGuid().ToString()
                            }
                        }
                    },
                    Token = encryptedToken,
                    PlayerCode = playerCode
                }
            };

            var client = _serviceProvider.GetRequiredService<IHabaneroApiClient>();
            var response = await client.FundTransferAsync(request);

            Set(SR.GameProviderCurrencyCode, gameProviderCurrencyCode);
            Set(SR.ExternalTransactionId, externalTransactionId);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.PlaceBetRequest, request);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        #endregion Private Method
    }
}