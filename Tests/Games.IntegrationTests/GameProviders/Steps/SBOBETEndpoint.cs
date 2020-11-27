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
using TechTalk.SpecFlow.Assist;
using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;
using TGS.Core.DataServices.Bet.Extensions;
using TGS.Core.DataServices.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding]
    [Scope(Tag = "SBOBET")]
    public sealed class SBOBETEndpoint : BaseGameEndpoint
    {
        private readonly ServiceProvider _serviceProvider;
        private const string GameProviderCode = Consts.GameProvider.SBOBET;

        public SBOBETEndpoint(ScenarioContext context)
            : base(context)
        {
            var services = (IServiceCollection) new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddTgsRepository(configuration, "Tgs");
            services.AddBetRepository(configuration, "Bet");

            services.AddJson()
                .AddNats(configuration)
                .AddSecurity()
                .AddTgsLog()
                .AddApiClient()
                .AddRedis();

            var apiUrlSection = configuration.GetSection("ApiUrls");
            services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(c =>
                c.BaseAddress = new Uri(apiUrlSection.GetValue<string>("Operator")));

            services.AddHttpClient<ISBOBETApiClient, SBOBETApiClient>(c =>
                c.BaseAddress = new Uri(apiUrlSection.GetValue<string>("Tgs-SBO")));

            _serviceProvider = services.BuildServiceProvider();
        }

        [Given(@"player ""(.*)"" of brand ""(.*)"" login via ""(.*)""")]
        public Task GivenPlayerOfBrandLoginViaAsync(string username, string brandCode, PlatformType platformType) =>
            PlayerLoginAsync(_serviceProvider, username, platformType, brandCode);

        [Given(@"player ""(.*)"" of brand ""(.*)"" without login")]
        public async Task GivenPlayerOfBrandWithoutLoginAsync(string username, string brandCode)
        {
            var db = GetDb<ITgsRepository>(_serviceProvider);
            var playerCode = await
                (from p in db.Players
                    join b in db.Brands on p.BrandId equals b.Id
                    where p.Username == username && b.Code == brandCode
                    select p.PlayerCode).AsNoTracking().FirstAsync();

            await DeauthorizePlayerAsync(_serviceProvider, playerCode);

            Set(SR.BrandCode, brandCode);
            Set(SR.PlayerCode, playerCode);
            Set(SR.Username, username);
            Remove($"{SR.LoginTime}:{username}:{PlatformType.Desktop}");
            Remove($"{SR.LoginTime}:{username}:{PlatformType.Mobile}");
        }

        [Given(@"player logout")]
        public Task GivenPlayerLogoutAsync()
        {
            var username = Get<string>(SR.Username);
            var playerCode = Get<string>(SR.PlayerCode);

            Remove($"{SR.LoginTime}:{username}:{PlatformType.Desktop}");
            Remove($"{SR.LoginTime}:{username}:{PlatformType.Mobile}");

            return DeauthorizePlayerAsync(_serviceProvider, playerCode);
        }

        [Given(@"player having balance of \$(.*)")]
        public Task GivenPlayerHavingBalanceOfAsync(decimal balance) =>
            SetPlayerBalanceAsync(_serviceProvider, balance);

        [Given(@"game provider has status set to ""(.*)""")]
        public Task GivenGameProviderHasStatusSetToAsync(Status status) => 
            ChangeGameProviderStatusAsync(_serviceProvider, GameProviderCode, status);

        [Given(@"game provider configuration has status set to ""(.*)""")]
        public Task GivenGameProviderConfigurationHasStatusSetToAsync(Status status) => 
            ChangeGameProviderConfigurationStatusAsync(_serviceProvider, GameProviderCode, status);

        [Given(@"game ""(.*)"" has status set to ""(.*)""")]
        public Task GivenGameHasStatusSetToAsync(string externalGameId, GameStatus gameStatus)
        {
            Set(SR.ExternalGameId, externalGameId);
            return ChangeGameStatusAsync(_serviceProvider, GameProviderCode, externalGameId, gameStatus);
        }

        [Given(@"brand has status set to ""(.*)""")]
        public Task GivenBrandHasStatusSetToAsync(Status status) => 
            ChangeBrandStatusAsync(_serviceProvider, status);

        [Given(@"balance now being \$(.*)")]
        public void GivenBalanceNowBeing(decimal balance) =>
            VerifyResponseWithoutError<BaseResponse>(balance);

        [When(@"I request the player balance")]
        public async Task WhenIRequestThePlayerBalanceAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var config = await GetSBOSetting();

            var client = _serviceProvider.GetRequiredService<ISBOBETApiClient>();

            var response = await client.GetPlayerBalanceAsync(new GetBalanceRequest
            {
                CompanyKey = config.CompanyKey,
                PlayerCode = playerCode,
                GameType = 1,
                ProductType = 1
            });

            Set(SR.Response, response);
        }

        [Given(@"I place bet of \$(.*) for game ""(.*)""")]
        [When(@"I place bet of \$(.*) for game ""(.*)""")]
        public async Task WhenIPlaceBetOfForGameAsync(decimal amount, string externalGameId)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var config = await GetSBOSetting();

            var types = externalGameId.SplitByUnderscore().Select(int.Parse).ToArray();

            var externalTransactionId = Guid.NewGuid().ToString();
            var externalRoundId = externalTransactionId;
            var client = _serviceProvider.GetRequiredService<ISBOBETApiClient>();

            var request = new PlaceBetRequest
            {
                CompanyKey = config.CompanyKey,
                PlayerCode = playerCode,
                GameType = types[1],
                ProductType = types[0],
                ExternalTransactionId = externalTransactionId,
                ExternalRoundId = externalTransactionId,
                Amount = amount,
                BetTime = DateTimeOffset.Now
            };

            var response = await client.PlaceBetAsync(request);

            Set(SR.ExternalTransactionId, externalTransactionId);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.PlaceBetRequest, request);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [Given(@"I win bet for \$(.*)")]
        [When(@"I win bet for \$(.*)")]
        public Task WhenIWinBetForAsync(decimal amount) => 
            SettleBetAsync(GameActionType.WinBet, amount);

        [Given(@"I lose bet")]
        [When(@"I lose bet")]
        public Task WhenILoseBetAsync() => 
            SettleBetAsync(GameActionType.LoseBet, 0);

        [Given(@"I tie bet for \$(.*)")]
        [When(@"I tie bet for \$(.*)")]
        public Task WhenITieBetForAsync(decimal amount) =>
            SettleBetAsync(GameActionType.TieBet, amount);

        [Given(@"I cancel transaction")]
        [When(@"I cancel transaction")]
        public async Task WhenICancelTransactionAsync()
        {
            var betRequest = Get<PlaceBetRequest>(SR.PlaceBetRequest);

            await CancelTransactionAsync(betRequest);

            var externalTransactionId = betRequest.ExternalTransactionId;
            var externalRoundId = betRequest.ExternalRoundId;
            var cancelId = $"{externalTransactionId}_cancel";

            Set(SR.ExternalTransactionId, cancelId);
            Set(SR.ExternalRoundId, externalRoundId);
        }

        [Given(@"I cancel all transactions")]
        [When(@"I cancel all transactions")]
        public async Task  WhenICancelAllTransactionsAsync()
        {
            var betRequest = Get<PlaceBetRequest>(SR.PlaceBetRequest);

            await CancelTransactionAsync(betRequest, true);

            var externalRoundId = betRequest.ExternalRoundId;
            Set(SR.ExternalRoundId, externalRoundId);
        }

        [Given(@"I cancel the settle transaction")]
        [When(@"I cancel the settle transaction")]
        public async Task WhenICancelTheSettleTransactionAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var config = await GetSBOSetting();
            
            var betRequest = Get<PlaceBetRequest>(SR.PlaceBetRequest);

            var client = _serviceProvider.GetRequiredService<ISBOBETApiClient>();

            var request = new RollbackRequest
            {
                CompanyKey = config.CompanyKey,
                PlayerCode = playerCode,
                GameType = betRequest.GameType,
                ProductType = betRequest.ProductType,
                ExternalTransactionId = betRequest.ExternalTransactionId,
            };

            var response = await client.RollbackAsync(request);

            var externalRoundId = betRequest.ExternalRoundId;
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [Given(@"I cancel the cancellation")]
        [When(@"I cancel the cancellation")]
        public async Task WhenICancelTheCancellation()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var config = await GetSBOSetting();
            
            var betRequest = Get<PlaceBetRequest>(SR.PlaceBetRequest);

            var client = _serviceProvider.GetRequiredService<ISBOBETApiClient>();

            var request = new RollbackRequest
            {
                CompanyKey = config.CompanyKey,
                PlayerCode = playerCode,
                GameType = betRequest.GameType,
                ProductType = betRequest.ProductType,
                ExternalTransactionId = betRequest.ExternalTransactionId,
            };

            var response = await client.RollbackAsync(request);

            var externalRoundId = betRequest.ExternalRoundId;
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [When(@"I resend the win bet")]
        public Task WhenIResendTheWinBetAsync() =>
            ResentSettleBetAsync();

        [When(@"I resend the lose bet")]
        public Task WhenIResendTheLoseBetAsync() =>
            ResentSettleBetAsync();

        [When(@"I resend the tie bet")]
        public Task WhenIResendTheTieBetAsync() =>
            ResentSettleBetAsync();

        [When(@"I resend cancel transaction")]
        public async Task WhenIResendCancelTransactionAsync()
        {
            var request = Get<CancelBetRequest>(SR.Request);

            var client = _serviceProvider.GetRequiredService<ISBOBETApiClient>();
            var response = await client.CancelBetAsync(request);

            Set(SR.Response, response);
        }

        [When(@"I resend cancel settlement transaction")]
        public async Task WhenIResendCancelSettlementTransactionAsync()
        {
            var request = Get<RollbackRequest>(SR.Request);

            var client = _serviceProvider.GetRequiredService<ISBOBETApiClient>();
            var response = await client.RollbackAsync(request);

            Set(SR.Response, response);
        }

        [When(@"I resend cancel the cancellation transaction")]
        public async Task WhenIResendCancelTheCancellationTransactionAsync()
        {
            var request = Get<RollbackRequest>(SR.Request);

            var client = _serviceProvider.GetRequiredService<ISBOBETApiClient>();
            var response = await client.RollbackAsync(request);

            Set(SR.Response, response);
        }

        [When(@"I get bet status")]
        public async Task WhenIGetBetStatusAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var config = await GetSBOSetting();
            
            var betRequest = Get<PlaceBetRequest>(SR.PlaceBetRequest);

            var client = _serviceProvider.GetRequiredService<ISBOBETApiClient>();

            var request = new GetBetStatusRequest
            {
                CompanyKey = config.CompanyKey,
                PlayerCode = playerCode,
                GameType = betRequest.GameType,
                ProductType = betRequest.ProductType,
                ExternalTransactionId = betRequest.ExternalTransactionId,
                ExternalRoundId = betRequest.ExternalRoundId
            };

            var response = await client.GetBetStatusAsync(request);

            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [Then(@"I will receive bet response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveBetResponseOfNoErrorAndBalanceOf(decimal expectedBalance) =>
            VerifyResponseWithoutError<PlaceBetResponse>(expectedBalance);

        [Then(@"I will receive settle bet response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveSettleBetResponseOfNoErrorAndBalanceOf(decimal expectedBalance) =>
            VerifyResponseWithoutError<SettleBetResponse>(expectedBalance);

        [Then(@"I will receive cancel response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveCancelResponseOfNoErrorAndBalanceOf(decimal expectedBalance) =>
            VerifyResponseWithoutError<CancelBetResponse>(expectedBalance);

        [Then(@"I will receive rollback response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveRollbackResponseOfNoErrorAndBalanceOf(decimal expectedBalance) =>
            VerifyResponseWithoutError<RollbackResponse>(expectedBalance);

        [Then(@"player balance response will be for \$(.*)")]
        public void ThenPlayerBalanceResponseWillBeFor(decimal expectedBalance) =>
            VerifyResponseWithoutError<GetBalanceResponse>(expectedBalance);

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

            var db = GetDb<IBetRepository>(_serviceProvider);
            var ga = await (
                from act in db.GameActions
                join r in db.Rounds on act.RoundId equals r.Id
                where r.ExternalRoundId == externalRoundId && act.ExternalTransactionId == externalTransactionId
                select act).AsNoTracking().FirstAsync();

            Set(SR.GameAction, ga);

            ga.Should().NotBeNull();
            ga.Type.Should().Be(type);
            ga.Amount.Should().Be(amount);
        }

        [Then(@"game platform will be recorded in the database of type ""(.*)""")]
        public async Task ThenGamePlatformWillBeRecordedInTheDatabaseOfTypeAsync(PlatformType platformType)
        {
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var db = GetDb<IBetRepository>(_serviceProvider);
            var platform = await db.Rounds
                .Where(r => r.ExternalRoundId == externalRoundId)
                .AsNoTracking()
                .Select(r => r.PlatformType)
                .FirstAsync();

            platform.Should().Be(platformType);
        }

        [Then(@"the last settle game action recorded in the database will be canceled")]
        public async Task ThenTheLastSettleGameActionRecordedInTheDatabaseWillBeCanceledAsync()
        {
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var db = GetDb<IBetRepository>(_serviceProvider);

            var gameActions = await (
                from act in db.GameActions
                join r in db.Rounds on act.RoundId equals r.Id
                where r.ExternalRoundId == externalRoundId
                select act).AsNoTracking().ToArrayAsync();

            var lastSettleId = gameActions
                .Where(ga => ga.Type.IsNotAllOf(GameActionType.PlaceBet, GameActionType.CancelTransaction))
                .Select(ga => ga.Id).First();

            var isCanceled = gameActions.Any(ga =>
                ga.Type == GameActionType.CancelTransaction && ga.ReferencedGameActionId == lastSettleId);

            isCanceled.Should().BeTrue();
        }

        [Then(@"the cancellation of game action recorded in the database will be canceled")]
        public async Task  ThenTheCancellationOfGameActionRecordedInTheDatabaseWillBeCanceledAsync()
        {
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var db = GetDb<IBetRepository>(_serviceProvider);

            var gameActions = await (
                from act in db.GameActions
                join r in db.Rounds on act.RoundId equals r.Id
                where r.ExternalRoundId == externalRoundId
                select act).AsNoTracking().ToArrayAsync();

            var last = gameActions.Last();
            var isCanceled = gameActions.Any(ga => ga.Type == GameActionType.CancelTransaction && ga.Id == last.ReferencedGameActionId);

            isCanceled.Should().BeTrue();
        }

        [Then(@"the round has no valid bets")]
        public async Task ThenTheRoundHasNoValidBetsAsync()
        {
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var db = GetDb<IBetRepository>(_serviceProvider);

            var gameActions = await (
                from act in db.GameActions
                join r in db.Rounds on act.RoundId equals r.Id
                where r.ExternalRoundId == externalRoundId
                select act).ToArrayAsync();

            var canceledActionIds = gameActions
                .Where(ga => ga.Type == GameActionType.CancelTransaction)
                .SelectMany(ga => new[] { ga.Id, ga.ReferencedGameActionId })
                .ToArray();

            var allCanceled = gameActions.All(ga => canceledActionIds.Contains(ga.Id));
            allCanceled.Should().BeTrue();
        }

        [Then(@"I will receive bet response of error ""(.*)""")]
        public void ThenIWillReceiveBetResponseOfError(GameApiError error) =>
            VerifyResponseWithError<PlaceBetResponse>(error);
        
        [Then(@"I will receive settle bet response of error ""(.*)""")]
        public void ThenIWillReceiveSettleBetResponseOfError(GameApiError error) =>
            VerifyResponseWithError<SettleBetResponse>(error);

        [Then(@"I will receive cancel bet response of error ""(.*)""")]
        public void ThenIWillReceiveCancelBetResponseOfError(GameApiError error) => 
            VerifyResponseWithError<CancelBetResponse>(error);

        [Then(@"I will receive rollback response of error ""(.*)""")]
        public void ThenIWillReceiveRollbackResponseOfError(GameApiError error) => 
            VerifyResponseWithError<RollbackResponse>(error);

        [Then(@"I will receive bet response of no error")]
        public void ThenIWillReceiveBetResponseOfNoError(Table table)
        {
            var (status, stake, winloss) = table.CreateInstance<(string status, decimal stake, decimal winloss)>();

            var betRequest = Get<PlaceBetRequest>(SR.PlaceBetRequest);
            var response = Get<GetBetStatusResponse>(SR.Response);

            response.ErrorCode.Should().Be((int) GameApiError.Success);
            response.ExternalTransactionId.Should().Be(betRequest.ExternalTransactionId);
            response.ExternalRoundId.Should().Be(betRequest.ExternalRoundId);
            response.Status.Should().Be(status);
            response.Stake.Should().Be(stake);
            response.WinLoss.Should().Be(winloss);
        }

        [Then(@"the round will be closed")]
        public Task ThenTheRoundWillBeClosedAsync() =>
            VerifyRoundStatus(_serviceProvider, RoundStatus.Closed);

        [Then(@"the round will be opened")]
        public Task ThenTheRoundWillBeOpened() => 
            VerifyRoundStatus(_serviceProvider, RoundStatus.Open);

        #region After Scenario

        [After]
        public Task AfterChangeGameProviderStatusScenarioAsync() => 
            ChangeGameProviderStatusAsync(_serviceProvider, GameProviderCode, Status.Active);

        [After]
        public Task AfterChangeGameProviderConfigurationStatusScenarioAsync() => 
            ChangeGameProviderConfigurationStatusAsync(_serviceProvider, GameProviderCode, Status.Active);
        
        [After]
        public Task AfterChangeGameStatusScenarioAsync() => 
            ChangeGameStatusAsync(_serviceProvider, GameProviderCode, Get<string>(SR.ExternalGameId), GameStatus.Active);
        
        [After]
        public Task AfterChangeBrandStatusScenarioAsync() => 
            ChangeBrandStatusAsync(_serviceProvider, Status.Active);
        
        #endregion

        #region Private Methods

        private Task<SBOConfiguration> GetSBOSetting() =>
            GetSettingAsync<SBOConfiguration>(_serviceProvider, SettingConsts.GameProviderConfigurations.SBOBET);

        private void VerifyResponseWithoutError<T>(decimal expectedBalance) where T : BaseResponse
        {
            var response = Get<T>(SR.Response);

            response.Should().NotBeNull();
            response.Balance.Should().Be(expectedBalance);
        }

        private void VerifyResponseWithError<T>(GameApiError error) where T : BaseResponse
        {
            var response = Get<T>(SR.Response);

            response.Should().NotBeNull();
            response.ErrorCode.Should().Be((int) error);
            response.Balance.Should().BeNull();
        }

        private async Task SettleBetAsync(GameActionType type, decimal amount)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var config = await GetSBOSetting();

            var betRequest = Get<PlaceBetRequest>(SR.PlaceBetRequest);

            var externalTransactionId = betRequest.ExternalTransactionId;
            var externalRoundId = betRequest.ExternalRoundId;

            var result = type switch
            {
                GameActionType.WinBet => ResultType.Win,
                GameActionType.FreeBet => ResultType.Win,
                GameActionType.LoseBet => ResultType.Loss,
                GameActionType.TieBet => ResultType.Tie,
                _ => throw new InvalidOperationException($"Invalid game action type '{type}' for settle bet")
            };

            var client = _serviceProvider.GetRequiredService<ISBOBETApiClient>();

            var request = new SettleBetRequest
            {
                CompanyKey = config.CompanyKey,
                PlayerCode = playerCode,
                GameType = betRequest.GameType,
                ProductType = betRequest.ProductType,
                ExternalTransactionId = betRequest.ExternalTransactionId,
                WinLoss = amount,
                Result = result,
                SettleTime = DateTimeOffset.Now
            };

            var response = await client.SettleBetAsync(request);

            var settleId = $"{externalTransactionId}_settle";

            Set(SR.ExternalTransactionId, settleId);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        private async Task ResentSettleBetAsync()
        {
            var request = Get<SettleBetRequest>(SR.Request);

            var client = _serviceProvider.GetRequiredService<ISBOBETApiClient>();
            var response = await client.SettleBetAsync(request);

            Set(SR.Response, response);
        }

        private async Task CancelTransactionAsync(PlaceBetRequest betRequest, bool isVoid = false)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var config = await GetSBOSetting();

            var client = _serviceProvider.GetRequiredService<ISBOBETApiClient>();

            var request = new CancelBetRequest
            {
                CompanyKey = config.CompanyKey,
                PlayerCode = playerCode,
                GameType = betRequest.GameType,
                ProductType = betRequest.ProductType,
                ExternalTransactionId = betRequest.ExternalTransactionId,
                ExternalRoundId = betRequest.ExternalRoundId,
                IsCancelAll = isVoid
            };

            var response = await client.CancelBetAsync(request);

            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        #endregion
    }
}