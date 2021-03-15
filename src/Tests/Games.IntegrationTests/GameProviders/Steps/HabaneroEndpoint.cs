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
using NSubstitute;
using TechTalk.SpecFlow;
using TGS.Apis.OperatorApi;
using TGS.Apis.TgsApi.GameProviders.Habanero;
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
using TGS.Core.Models.Tokens;
using TGS.Shared.Game.Models.Habanero.Incoming;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;

namespace TGS.Tests.Games.IntegrationTests.GameProviders.Steps
{
    [Binding]
    [Scope(Tag = "Habanero")]
    public sealed class HabaneroEndpoint : BaseGameEndpoint
    {
        private const string GameProviderCode = Consts.GameProvider.Habanero;

        private readonly Authorization _authorization = new()
        {
            HabaneroBrandId = "a5df0ada-7b6a-eb11-9889-00155db5435a",
            ClientId = "habanero",
            SecretKey = "slPrV84jbxM8jIH9nUYBAndgVyjF5nU6",
            MachineName = "TESTSERVER",
            Culture = "en"
        };

        private readonly ServiceProvider _serviceProvider;

        public HabaneroEndpoint(ScenarioContext context)
            : base(context)
        {
            Environment.SetEnvironmentVariable("GameProviderCode", GameProviderCode);

            var fakeAnalyzer = Substitute.For<IRequestAnalyzer>();
            fakeAnalyzer.GetIp(Arg.Any<HttpContext>()).Returns("127.0.0.1");

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
                            srv.AddTransient(_ => fakeAnalyzer);
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
                        .AddJsonFile("GameProviders/Habanero/habanero.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<HabaneroStartup>()
                        .UseKestrel(o => o.AddServerHeader = false)
                        .UseTestServer()
                        .ConfigureTestServices(srv =>
                        {
                            srv.AddTransient(_ => fakeAnalyzer);
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

            services.AddTgsRepository(configuration, "Tgs")
                .AddJson()
                .AddNats(configuration)
                .AddSecurity()
                .AddTgsLog()
                .AddApiClient()
                .AddRedis();

            var isDev = configuration.GetValue<bool>("ApiUrls:IsDev");

            if (isDev)
            {
                services.AddTransient<IPlayerApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<PlayerApiClient>(sp, opApiClient));

                services.AddTransient<IHabaneroApiClient>(sp =>
                    ActivatorUtilities.CreateInstance<HabaneroApiClient>(sp, tgsApiClient));
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

        [Given(@"player having balance of \$(.*)")]
        public Task GivenPlayerHavingBalanceOfAsync(decimal balance) =>
            SetPlayerBalanceAsync(_serviceProvider, balance);

        [When(@"I call authorize endpoint with username=""(.*)"" and passkey=""(.*)"" and brandid=""(.*)""")]
        public async Task WhenICallDetailsEndpointWithUsernameAndPasswordAndPlayerAuthToken(string clientId,
            string secretKey, string habaneroBrandId)
        {
            var encryptedToken = await GetEncryptedTokenAsync();
            var client = _serviceProvider.GetRequiredService<IHabaneroApiClient>();

            var response = await client.AuthorizePlayerAsync(new()
            {
                Type = "playerdetailrequest",
                SentOn = DateTimeOffset.Now,
                Game = new()
                {
                    ExternalRtpId = "af834712-47b4-4063-b63b-a826979a01e1",
                    ExternalId = "AcesandEights1Hand"
                },
                Authorization = new()
                {
                    ClientId = clientId,
                    SecretKey = secretKey,
                    HabaneroBrandId = habaneroBrandId,
                    MachineName = "TESTSERVER"
                },
                Detail = new()
                {
                    Token = encryptedToken,
                    GameLaunch = true
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

        #region Private Method

        private Task<HabaneroConfiguration> GetHabaneroSetting() =>
            GetSettingAsync<HabaneroConfiguration>(_serviceProvider, SettingConsts.GameProviderConfigurations.Habanero);

        private Task<string> GetEncryptedTokenAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var key = $"{SR.PlayerTokenData}:{playerCode}:{platform}";
            var tokenData = Get<PlayerTokenData>(key);

            var tokenProvider = _serviceProvider.GetRequiredService<ITokenProvider>();
            return tokenProvider.EncryptAsync(tokenData.LoginRecordId);
        }

        #endregion
    }
}