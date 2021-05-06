using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
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
                    PlayerCode = playerCode
                }
            };

            var client = _serviceProvider.GetRequiredService<ISabaApiClient>();
            var response = await client.GetBalanceAsync(request).ConfigureAwait(false);

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

        #region Private Methods

        private async ValueTask<SabaConfiguration> GetConfigurationAsync() =>
            await GetSettingAsync<SabaConfiguration>(_serviceProvider,
                SettingConsts.GameProviderConfigurations.Saba).ConfigureAwait(false);

        #endregion
    }
}