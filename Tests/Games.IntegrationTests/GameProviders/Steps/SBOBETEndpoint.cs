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
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;
using TGS.Core.DataServices.Bet.Extensions;
using TGS.Core.DataServices.Bet.Models;
using TGS.Core.DataServices.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
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
                .AddApiClient();

            var apiUrlSection = configuration.GetSection("ApiUrls");
            services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(c =>
                c.BaseAddress = new Uri(apiUrlSection.GetValue<string>("Operator")));

            services.AddHttpClient<ISBOBETApiClient, SBOBETApiClient>(c =>
                c.BaseAddress = new Uri(apiUrlSection.GetValue<string>("Tgs-SBO")));

            _serviceProvider = services.BuildServiceProvider();
        }

        [Given(@"player ""(.*)"" of licensee ""(.*)"" login via ""(.*)""")]
        public Task GivenPlayerLoginViaAsync(string username, string licenseeCode, PlatformType platformType) =>
            PlayerLoginAsync(username, platformType, licenseeCode, _serviceProvider);

        [Given(@"player ""(.*)"" of licensee ""(.*)"" without login")]
        public async Task GivenPlayerOfLicenseeWithoutLoginAsync(string username, string licenseeCode)
        {
            var db = GetDb<ITgsRepository>(_serviceProvider);
            var playerCode = await db.Players.Where(p => p.Username == username)
                .Select(p => p.PlayerCode).FirstAsync();

            Set(SR.PlayerCode, playerCode);
        }

        [Given(@"player having balance of \$(.*)")]
        public Task GivenPlayerHavingBalanceOfAsync(decimal balance) =>
            SetPlayerBalanceAsync(_serviceProvider, balance);

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
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        [Then(@"I will receive bet response of no error and balance of \$(.*)")]
        public void ThenIWillReceiveBetResponseOfNoErrorAndBalanceOf(decimal expectedBalance) => 
            VerifyResponseWithoutError<PlaceBetResponse>(expectedBalance);
        
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
                select act).FirstAsync();

            Set(SR.GameAction, ga);

            ga.Should().NotBeNull();
            ga.Type.Should().Be(type);
            ga.Amount.Should().Be(amount);
        }
        
        [Then(@"game platform will be recorded in the database of type ""(.*)""")]
        public async Task ThenGamePlatformWillBeRecordedInTheDatabaseOfTypeAsync(PlatformType platformType)
        {
            var externalRoundId = Get<string>(SR.ExternalRoundId);
            var betDb = GetDb<IBetRepository>(_serviceProvider);
            var platform = await betDb.Rounds.Where(r => r.ExternalRoundId == externalRoundId)
                .Select(r => r.PlatformType)
                .FirstAsync();

            platform.Should().Be(platformType);
        }


        #region Private Methods
        
        private Task<SBOConfiguration> GetSBOSetting() =>
            GetSettingAsync<SBOConfiguration>(_serviceProvider, SettingConsts.GameProviderConfigurations.SBOBET);

        
        private void VerifyResponseWithoutError<T>(decimal expectedBalance) where T :　BaseResponse
        {
            var response = Get<T>(SR.Response);

            response.Should().NotBeNull();
            response.Balance.Should().Be(expectedBalance);
        }

        #endregion
    }
}