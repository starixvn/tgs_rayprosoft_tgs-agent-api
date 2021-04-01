using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using TechTalk.SpecFlow;
using TGS.Apis.OperatorApi;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services;
using TGS.Core.DataServices.Tgs.Extensions;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Extensions;
using TGS.Shared.Game.Models.PragmaticPlay.Common;
using TGS.Shared.Game.Models.PragmaticPlay.Incoming;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding]
    [Scope(Tag = "PragmaticPlay")]
    public sealed class PragmaticPlayEndpoint : BaseGameEndpoint
    {
        private static readonly string GameProviderCode = Consts.GameProvider.PragmaticPlay;
        private readonly ServiceProvider _serviceProvider;

        public PragmaticPlayEndpoint(ScenarioContext context)
            : base(context)
        {
            Environment.SetEnvironmentVariable("GameProviderCode", GameProviderCode);

            var fakeAnalyzer = new Mock<IRequestAnalyzer>();
            fakeAnalyzer.Setup(x => x.GetIp(It.IsAny<HttpContext>())).Returns("127.0.0.1");

            var currentDictionary = Directory.GetCurrentDirectory();

            var opApiHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    var path = Path.Combine(currentDictionary, "../../../../../Apis/OperatorApi");
                    builder.SetBasePath(Path.GetFullPath(path));
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>()
                        .UseTestServer()
                        .ConfigureTestServices(srv =>
                        {
                            srv.AddTransient(_ => fakeAnalyzer.Object);
                        });
                })
                .Build();

            opApiHost.StartAsync();
            var opApiClient = opApiHost.GetTestClient();

            var tgsApiHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    var path = Path.Combine(currentDictionary, "../../../../../Apis/TgsApi");

                    builder.SetBasePath(Path.GetFullPath(path))
                        .AddJsonFile("GameProviders/PragmaticPlay/pragmaticplay.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<PragmaticPlayStartup>()
                        .UseKestrel(o => o.AddServerHeader = false)
                        .UseTestServer()
                        .ConfigureTestServices(srv =>
                        {
                            srv.AddTransient(_ => fakeAnalyzer.Object);
                        });
                })
                .Build();

            tgsApiHost.StartAsync();
            var tgsApiClient = tgsApiHost.GetTestClient();

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

            services.AddSingleton<IPragmaticPlayHashProvider, PragmaticPlayHashProvider>();

            services.AddTgsRepository(configuration, "Tgs")
                .AddJson()
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
                    ActivatorUtilities.CreateInstance<PlayerApiClient>(sp, opApiClient));

                services.AddTransient<IPragmaticPlayApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<PragmaticPlayApiClient>(sp, tgsApiClient));
            }
            else
            {
                services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Operator")));

                services.AddHttpClient<IPragmaticPlayApiClient, PragmaticPlayApiClient>(
                    c => c.BaseAddress = new(configuration.GetValue<string>("ApiUrls:Tgs-PragmaticPlay")));
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

        [Given(@"player logout")]
        public Task GivenPlayerLogoutAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            return DeauthorizePlayerAsync(_serviceProvider, playerCode);
        }

        [When(@"I call authorize endpoint")]
        public async Task WhenICallAuthorizeEndpoint()
        {
            var configuration = await GetConfigurationAsync();

            var model = new AuthorizePlayerRequest
            {
                Token = string.Empty,
                ProviderId = configuration.ProviderId
            };

            var hash = ComputeHash(model, configuration.SecretKey);
            var request = model with {Hash = hash};

            var client = _serviceProvider.GetRequiredService<IPragmaticPlayApiClient>();
            var response = await client.AuthorizePlayerAsync(request).ConfigureAwait(false);

            Set(SR.Response, response);
        }

        [Then(@"I will get authorize response and player balance will be \$(.*)")]
        public async Task ThenIWillGetAuthorizeResponseAndPlayerBalanceWillBe(decimal balance)
        {
            var wallet = await GetPlayerWalletAsync(_serviceProvider);

            var gameProviderOperations = _serviceProvider.GetRequiredService<IGameProviderOperations>();
            var gameProviderId = await gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderCode)
                .ConfigureAwait(false);

            var mapper = _serviceProvider.GetRequiredService<IGameProviderCurrencyCodeMapper>();
            var gameProviderCurrency = await mapper.InternalToExternalAsync(wallet.CurrencyCode, gameProviderId)
                .ConfigureAwait(false);

            var response = Get<AuthorizePlayerResponse>(SR.Response);

            response.Should().NotBeNull();
            response.Balance.Should().Be(balance);
            response.Currency.Should().Be(gameProviderCurrency);
        }

        [Then(@"I will get error authorize response and no player balance")]
        public void ThenIWillGetErrorAuthorizeResponseAndNoPlayerBalance() => ScenarioContext.Current.Pending();


        #region Private Members

        private async ValueTask<PragmaticPlayConfiguration> GetConfigurationAsync()
        {
            var settingOperations = _serviceProvider.GetRequiredService<ISettingOperations>();
            return await settingOperations
                .GetValueFromJsonByKeyAsync<PragmaticPlayConfiguration>(SettingConsts.GameProviderConfigurations
                    .PragmaticPlay).ConfigureAwait(false);
        }

        private string ComputeHash<T>(T model, string secretKey) where T : HashRequest
        {
            var provider = _serviceProvider.GetRequiredService<IPragmaticPlayHashProvider>();
            return provider.ComputeHash(model, secretKey);
        }

        #endregion
    }
}