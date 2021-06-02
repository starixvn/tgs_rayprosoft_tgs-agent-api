using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
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
using TGS.Shared.Game.Models.SexyGaming.Incoming;
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

            var response = await _client.GetBalanceAsyncstAsync(configuration.Cert, encryptedToken, data).ConfigureAwait(false);

            Set(SR.Response, response);
        }

        #region Private Methods

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
        #endregion
    }
}
