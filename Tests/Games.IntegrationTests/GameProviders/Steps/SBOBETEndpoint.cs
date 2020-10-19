using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.DataServices.Bet.Extensions;
using TGS.Core.DataServices.Extensions;
using TGS.Core.Infrastructures.ApiClient;
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

        [Given(@"player having balance of \$(.*)")]
        public Task GivenPlayerHavingBalanceOfAsync(decimal balance) =>
            SetPlayerBalanceAsync(_serviceProvider, balance);

        [When(@"I request the player balance")]
        public async Task WhenIRequestThePlayerBalanceAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);

            var config =
                await GetSettingAsync<SBOConfiguration>(_serviceProvider,
                    SettingConsts.GameProviderConfigurations.SBOBET);

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

        [Then(@"player balance response will be for \$(.*)")]
        public void ThenPlayerBalanceResponseWillBeFor(decimal balance)
        {
            var response = Get<GetBalanceResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Balance.Should().Be(balance);
        }
    }
}