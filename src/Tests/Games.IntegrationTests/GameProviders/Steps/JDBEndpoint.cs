// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

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
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.JDB.Enums;
using TGS.Shared.Game.Models.JDB.Incoming;
using TGS.Shared.Game.Models.JDB.Outgoing;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding]
    [Scope(Tag = "JDB")]
    public sealed class JDBEndpoint : BaseGameEndpoint
    {
        private const string GameProviderCode = Consts.GameProvider.JDB;
        private readonly ServiceProvider _serviceProvider;

        public JDBEndpoint(ScenarioContext context)
            : base(context)
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

            services.AddTgsRepository(configuration, "Tgs");

            services.AddSystemJsonSerializer()
                .AddNats(configuration)
                .AddSecurity()
                .AddTgsLog()
                .AddApiClient()
                .AddRedis();
            
            services.AddScoped<ISettingQueries, SettingQueries>();
            services.AddTransient<Func<ISettingQueries>>(c => c.GetRequiredService<SettingQueries>);
            services.AddScoped<ISettingOperations, SettingOperations>();
            services.AddSingleton<ISymmetricCryptoProviderFactory, SymmetricCryptoProviderFactory>();

            var apiUrlSection = configuration.GetSection("ApiUrls");
            var isDev = configuration.GetValue<bool>("ApiUrls:IsDev");

            if (isDev)
            {
                services.AddTransient<IPlayerApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<PlayerApiClient>(sp, Hooks.OpApiClient));
                services.AddTransient<IJDBApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<JDBApiClient>(sp, Hooks.TgsApiClient));
            }
            else
            {
                services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(c =>
                    c.BaseAddress = new(apiUrlSection.GetValue<string>("Operator")));

                services.AddHttpClient<IJDBApiClient, JDBApiClient>(c =>
                    c.BaseAddress = new(apiUrlSection.GetValue<string>("Tgs-JDB")));
            }

            _serviceProvider = services.BuildServiceProvider();
        }

        [Given(@"player ""(.*)"" of brand ""(.*)"" with ""(.*)"" login via ""(.*)""")]
        public Task GivenPlayerOfBrandLoginViaAsync(string username, string brandCode,
            string currencyCode, PlatformType platformType) =>
            PlayerLoginAsync(_serviceProvider, username, platformType, brandCode, currencyCode);

        [Given(@"player having balance of \$(.*)")]
        public Task GivenPlayerHavingBalanceOfAsync(decimal balance) =>
            SetPlayerBalanceAsync(_serviceProvider, balance);

        [Given(@"game ""(.*)"" has status set to ""(.*)""")]
        public Task GivenGameHasStatusSetToAsync(string externalGameId, GameStatus gameStatus)
        {
            externalGameId = $"{Get<int>(SR.JDBGameType)}_{externalGameId}";
            Set(SR.ExternalGameId, externalGameId);
            return ChangeGameStatusAsync(_serviceProvider, GameProviderCode, externalGameId, gameStatus);
        }
        
        [Given(@"game provider has status set to ""(.*)""")]
        public Task GivenGameProviderHasStatusSetToAsync(Status status) =>
            ChangeGameProviderStatusAsync(_serviceProvider, GameProviderCode, status);
        
        [Given(@"game provider configuration has status set to ""(.*)""")]
        public Task GivenGameProviderConfigurationHasStatusSetToAsync(Status status) =>
            ChangeGameProviderConfigurationStatusAsync(_serviceProvider, GameProviderCode, status);

        [Given(@"brand has status set to ""(.*)""")]
        public Task GivenBrandHasStatusSetToAsync(Status status) =>
            ChangeBrandStatusAsync(_serviceProvider, status);

        [Given(@"set game type as ""(.*)""")]
        public void GivenSetGameType(GameType gameType)
        {
            if (gameType == GameType.Slot)
            {
                Set(SR.JDBGameType, 0);
            }

            if (gameType == GameType.Fishing)
            {
                Set(SR.JDBGameType, 7);
            }
        }


        [When(@"I request the player balance")]
        public async Task WhenIRequestThePlayerBalanceAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);

            var client = _serviceProvider.GetRequiredService<IJDBApiClient>();

            var response = await client.GetPlayerBalanceAsync(new()
            {
               ActionType = ActionType.GetBalance,
               SystemTime= DateTime.UtcNow.Ticks,
               UserId = playerCode,
               Currency = "RB"
            });

            Set(SR.Response, response);
        }

        [Given(@"I place bet of \$(.*) for game ""(.*)"" and win \$(.*)")]
        [When(@"I place bet of \$(.*) for game ""(.*)"" and win \$(.*)")]
        public Task WhenIPlaceBetAndWinOfForGameAsync(decimal amount, string externalGameId, decimal win) =>
            PlaceBetAndWinAsync(amount, externalGameId, win);
        
        [Given(@"I place bet of \$(.*) for game ""(.*)"" and lose")]
        [When(@"I place bet of \$(.*) for game ""(.*)"" and lose")]
        public Task WhenIPlaceBetAndLoseOfForGameAsync(decimal amount, string externalGameId) =>
            PlaceBetAndWinAsync(amount, externalGameId, 0);


        [Then(@"I will receive bet response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveBetResponseOfNoErrorAndBalanceOf(decimal expectedBalance) =>
            VerifyResponseWithoutError<BaseActionResponse>(expectedBalance);

        [Then(@"player balance response will be for \$(.*)")]
        public void ThenPlayerBalanceResponseWillBeFor(decimal expectedBalance) =>
            VerifyResponseWithoutError<BaseActionResponse>(expectedBalance);

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
            if (type == GameActionType.WinBet || type == GameActionType.LoseBet)
            {
                externalTransactionId += "_settle";
            }

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

        [Then(@"I will receive bet response of error ""(.*)""")]
        public void ThenIWillReceiveBetResponseOfError(GameApiStatus error) =>
            VerifyResponseWithError<BaseActionResponse>(error);

        [Then(@"the round will be closed")]
        public Task ThenTheRoundWillBeClosedAsync() =>
            VerifyRoundStatus(_serviceProvider, RoundStatus.Closed);
        

        [When("I resend the same bet")]
        public async Task WhenIResendTheSameBet()
        {
            var request = Get<PlaceAndSettleBetRequest>(SR.PlaceBetRequest);
            var client = _serviceProvider.GetRequiredService<IJDBApiClient>();
            var response = await client.PlaceAndSettleBetAsync(request);
            Set(SR.Response, response);
        }

        [When("I void round")]
        public async Task WhenIVoidRound()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            
            var systemTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var client = _serviceProvider.GetRequiredService<IJDBApiClient>();
            var placeAndSettleRequest = Get<PlaceAndSettleBetRequest>(SR.Request) ??
                new PlaceAndSettleBetRequest {TransferId = systemTime};

            var request = new CancelTransactionRequest
            {
                ActionType = ActionType.CancelTransaction,
                SystemTime = systemTime,
                TransferId = placeAndSettleRequest.TransferId,
                UserId = playerCode
            };

            var response = await client.CancelTransactionsAsync(request);
            
            Set(SR.Request, request);
            Set(SR.Response, response);
        }
        
        [Then(
            @"all cancel bet related database records will be created and the balance of each user will be matched as original")]
        public async Task
            ThenAllCancelBetRelatedDatabaseRecordsWillBeCreatedAndTheBalanceOfEachUserWillBeMatchedAsOriginal()
        {
            var request = Get<PlaceAndSettleBetRequest>(SR.PlaceBetRequest);

            var db = GetDb<ITgsRepository>(_serviceProvider);
            var gameActions = await db.GameActions
                .AsQueryable()
                .Where(ga => ga.ExternalTransactionId.StartsWith(request.TransferId.ToString()))
                .Select(ga => new
                {
                    ga.Round.PlayerCode,
                    ga.Id
                }).ToArrayAsync();

            gameActions.Length.Should().Be(4);
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

        #endregion After Scenario

        #region Private Methods

        private async Task PlaceBetAndWinAsync(decimal amount, string externalGameId, decimal win)
        {
            var playerCode = Get<string>(SR.PlayerCode);

            var now = DateTimeOffset.Now;
            var systemTime = now.ToUnixTimeMilliseconds();
            int.TryParse(externalGameId, out var machineType);
            var client = _serviceProvider.GetRequiredService<IJDBApiClient>();

            var request = new PlaceAndSettleBetRequest
            {
                ActionType = ActionType.PlaceAndSettleBet,
                SystemTime = systemTime,
                TransferId = systemTime,
                GameSequenceNo = systemTime,
                UserId = playerCode,
                GameType = Get<int>(SR.JDBGameType),
                MachineType = machineType,
                ReportDate = now.ToString("dd-MM-yyyy"),
                GameDate = now.ToString("dd-MM-yyyy HH:mm:ss"),
                Currency = "RB",
                Bet = amount,
                Win = win,
                NetWin = win - amount,
                Denom = 0.01m,
                IpAddress = "127.0.0.1",
                ClientType = "Web",
                SystemTakeWin = 0,
                LastModifyTime =  now.ToString("dd-MM-yyyy HH:mm:ss"),
                RoomType = 0
            };

            var response = await client.PlaceAndSettleBetAsync(request);

            Set(SR.ExternalTransactionId, request.TransferId.ToString());
            Set(SR.ExternalRoundId, request.GameSequenceNo.ToString());
            externalGameId = $"{Get<int>(SR.JDBGameType)}_{externalGameId}";
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.PlaceBetRequest, request);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        private void VerifyResponseWithoutError<T>(decimal expectedBalance) where T : BaseActionResponse
        {
            var response = Get<T>(SR.Response);

            response.Should().NotBeNull();
            response.Balance.Should().Be(expectedBalance);
        }

        private void VerifyResponseWithError<T>(GameApiStatus error) where T : BaseActionResponse
        {
            var response = Get<T>(SR.Response);

            response.Should().NotBeNull();
            response.ErrorMessage.Should().NotBeNullOrEmpty();
            response.Status.Should().Be(error.Description());
        }

        #endregion Private Methods
    }
}