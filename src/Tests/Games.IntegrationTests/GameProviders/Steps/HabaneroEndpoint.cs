using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using TechTalk.SpecFlow;
using TGS.Apis.OperatorApi;
using TGS.Apis.TgsApi.GameProviders.Habanero;
using TGS.Core.DataServices.Tgs;
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
using TGS.Shared.Game.Models.Habanero.Enums;
using TGS.Shared.Game.Models.Habanero.Incoming;
using TGS.Tests.ApiClients.GameProviders;
using TGS.Tests.ApiClients.Operators;
using TGS.Tests.TestsCommon;
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
            SecretKey = "slPrV84jbxM8jIH9nUYBAndgVyjF5nU6",
            MachineName = "TESTSERVER",
            Culture = "en"
        };

        private readonly ServiceProvider _serviceProvider;

        public HabaneroEndpoint(ScenarioContext context)
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
                        .AddJsonFile("GameProviders/Habanero/habanero.json", true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<HabaneroStartup>()
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
        }

        [Given(@"player logout")]
        public Task GivenPlayerLogoutAsync()
        {
            var playerCode = Get<string>(SR.PlayerCode);
            return DeauthorizePlayerAsync(_serviceProvider, playerCode);
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
            var placeBetRequest = Get<FundTransferRequest>(SR.PlaceBetRequest);
            var response = Get<FundTransferResponse>(SR.Response);


            response.Should().NotBeNull();
            response.Result.Status.Success.Should().BeTrue();
            response.Result.Status.DebitSucceeded.Should().BeTrue();
            response.Result.Status.CreditSucceeded.Should().BeTrue();
            response.Result.Balance.Should().Be(expectedBalance);
            //response.Result.CurrencyCode.Should().Be(TestData.PlayerCurrency);
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

        #region Private Method

        private Task<HabaneroConfiguration> GetHabaneroSetting() =>
            GetSettingAsync<HabaneroConfiguration>(_serviceProvider, SettingConsts.GameProviderConfigurations.Habanero);

        private Task<string> GetEncryptedTokenAsync(string playerCode, PlatformType platform)
        {
            var key = $"{SR.PlayerTokenData}:{playerCode}:{platform}";
            var tokenData = Get<PlayerTokenData>(key);

            var tokenProvider = _serviceProvider.GetRequiredService<ITokenProvider>();
            return tokenProvider.EncryptAsync(tokenData.LoginRecordId);
        }

        private async Task PlaceBetAsync(decimal amount, string externalGameId, string externalRoundId = null)
        {
            var playerCode = Get<string>(SR.PlayerCode);
            var platform = Get<PlatformType>(SR.PlatformType);
            var currencyCode = Get<string>(SR.CurrencyCode);
            var encryptedToken = await GetEncryptedTokenAsync(playerCode, platform);

            externalRoundId ??= NewRoundId();
            var externalTransactionId = externalRoundId;
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
                    RoundId = externalRoundId,
                    ClientRoundId = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
                    IsCancel = false,
                    IsRetry = false,
                    RetryCount = 0,
                    IsSettlementRetry = false,
                    FundTransfer = new()
                    {
                        IsBetAndSettle = false,
                        Transactions = new[]
                        {
                            new Transaction
                            {
                                GameState = GameState.NewGame,
                                TransactionId = externalRoundId,
                                CurrencyCode = currencyCode,
                                Amount = -amount,
                                TransactionReferenceId = externalRoundId,
                                Timestamp = now.ToString(TimeFormat)
                            }
                        }
                    },
                    Token = encryptedToken,
                    PlayerCode = playerCode
                }
            };

            var response = await client.FundTransferAsync(request);

            Set(SR.ExternalTransactionId, externalTransactionId);
            Set(SR.ExternalRoundId, externalRoundId);
            Set(SR.ExternalGameId, externalGameId);
            Set(SR.PlaceBetRequest, request);
            Set(SR.Request, request);
            Set(SR.Response, response);
        }

        private string NewRoundId() =>
            Guid.NewGuid().ToString();

        #endregion
    }
}