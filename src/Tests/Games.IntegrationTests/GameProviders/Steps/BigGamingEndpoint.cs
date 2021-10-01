using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TechTalk.SpecFlow;
using Tgs.Logging;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.BigGaming;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations.Extensions;
using TGS.Shared.Game.Models.BigGaming.Commons;
using TGS.Shared.Game.Models.BigGaming.Incoming;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding]
    [Scope(Tag = "BigGaming")]
    public class BigGamingEndpoint : BaseGameEndpoint
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IBigGamingApiClient _client;
        private readonly IHashProvider _hashProvider;

        public BigGamingEndpoint(ScenarioContext context)
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

            services.AddSingleton<ITgsLog>(new Mock<ITgsLog>().Object);

            var isDev = configuration.GetValue<bool>("ApiUrls:IsDev");

            if (isDev)
            {
                services.AddTransient<IPlayerApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<PlayerApiClient>(sp, Hooks.OpApiClient));

                services.AddTransient<IBigGamingApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<BigGamingApiClient>(sp, Hooks.TgsApiClient));
            }
            else
            {
                services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Operator")));

                services.AddHttpClient<IBigGamingApiClient, BigGamingApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Tgs-BigGaming")));
            }

            _serviceProvider = services.BuildServiceProvider();
            _client = _serviceProvider.GetRequiredService<IBigGamingApiClient>();
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
            var response = Get<BigGamingResponse>(SR.Response);

            decimal balance = JsonSerializer.Deserialize<decimal>(JsonSerializer.Serialize(response.Result));
            response.Error.Should().BeNull();
            balance.Should().Be(expectedBalance);
        }


        [When(@"I request the player balance")]
        public async Task WhenIRequestThePlayerBalanceAsync()
        {
            var currencyCode = Get<string>(SR.CurrencyCode);

            var playerCode = Get<string>(SR.PlayerCode);

            var configuration = await GetBrandMerchant(currencyCode).ConfigureAwait(false);

            string random = GuidExtensions.NewSequentialGuid().ToString();
            var data = new BigGamingBalanceRequest()
            {
                Random = random,
                Sign = _hashProvider.Hash($"{random }{configuration.SecretCode}{playerCode}{configuration.SecretKey}", HashAlgorithmKind.Md5).ToUpper(),
                LoginId = playerCode
            };


            var request = new BigGamingRequest
            {
                Id = random,
                Method = "open.operator.user.balance",
                Params = JsonDocument.Parse(JsonSerializer.Serialize(data)),
                Jsonrpc = "2.0"
            };


            var response = await _client.EntranceAsyncstAsync(request).ConfigureAwait(false);

            Set(SR.Response, response);
        }

        #region Private Methods

        private async ValueTask<BigGamingConfiguration> GetConfigurationAsync() =>
            await GetSettingAsync<BigGamingConfiguration>(_serviceProvider,
                SettingConsts.GameProviderConfigurations.BigGaming).ConfigureAwait(false);

        private Guid GetLoginRecordId(string playerCode, PlatformType platform)
        {
            var key = $"{SR.PlayerTokenData}:{playerCode}:{platform}";
            var tokenData = Get<PlayerTokenData>(key);
            return tokenData?.LoginRecordId ?? Guid.Empty;
        }

        private async ValueTask<BigGamingBrandMerchant> GetBrandMerchant(string currencyCode)
        {
            var configuration =
              await GetSettingAsync<BigGamingConfiguration>(_serviceProvider,
                SettingConsts.GameProviderConfigurations.BigGaming).ConfigureAwait(false);

            return configuration.BrandMerchants.Where(x => x.CurrencyCode.IgnoreCaseEquals(currencyCode) && x.Status).First();
        }


        #endregion
    }
}
