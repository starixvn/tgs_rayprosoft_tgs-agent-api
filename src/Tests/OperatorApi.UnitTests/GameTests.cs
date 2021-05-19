using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Handlers.Game;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;
using TGS.Services.PluggableServices;
using PluggableModels = TGS.Services.PluggableServices.Models;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public async Task Get_Game_List_Of_Specific_GameProvider()
        {
            // arrange
            var fakeHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeLocalizationOperations = Substitute.For<ILocalizationOperations>();
            var fakeBrandLanguageCodeMapper = Substitute.For<IBrandLanguageCodeMapper>();
            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakeRedisCacheManager = Substitute.For<IRedisCacheManager>();
            var fakeJsonSerializationProvider = Substitute.For<IJsonSerializationProvider>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();

            var sut = new GetGamesHandler(
                fakeHttpContextAccessor,
                fakeBrandOperations,
                fakeGameProviderOperations,
                fakePlayerOperations,
                fakeLocalizationOperations,
                fakeBrandLanguageCodeMapper,
                fakeBrandCurrencyCodeMapper,
                fakeRedisCacheManager,
                fakeJsonSerializationProvider,
                fakeValidStatus,
                fakeValidGameStatus);

            // act
            GetGamesRequest request = new GetGamesRequest()
            {
                GameProviderCode = "SBO",
                PlatformType = PlatformType.Desktop,
                CurrencyCode = "CNY",
                LanguageCode = "zh-TW",
                Types = Array.Empty<GameType>()
            };

            await sut.Handle(request, default);

            // assert
            // 驗證有傳gpcode時 會執行一次 _gameProviderOperations.GetGameProviderIdByCodeAsync 取得gameProviderId
            _ = fakeGameProviderOperations.Received(1).GetGameProviderIdByCodeAsync(Arg.Is(request.GameProviderCode));
        }

        [Test]
        public async Task Get_Game_List_Of_All_GameProviders()
        {
            // arrange
            var fakeHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeLocalizationOperations = Substitute.For<ILocalizationOperations>();
            var fakeBrandLanguageCodeMapper = Substitute.For<IBrandLanguageCodeMapper>();
            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakeRedisCacheManager = Substitute.For<IRedisCacheManager>();
            var fakeJsonSerializationProvider = Substitute.For<IJsonSerializationProvider>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();

            var brandId = Guid.NewGuid();

            var httpContext = new DefaultHttpContext();
            httpContext.Items.Add(Consts.ContextItems.BrandId, brandId);

            fakeHttpContextAccessor.HttpContext.Returns(httpContext);

            var sut = new GetGamesHandler(
                fakeHttpContextAccessor,
                fakeBrandOperations,
                fakeGameProviderOperations,
                fakePlayerOperations,
                fakeLocalizationOperations,
                fakeBrandLanguageCodeMapper,
                fakeBrandCurrencyCodeMapper,
                fakeRedisCacheManager,
                fakeJsonSerializationProvider,
                fakeValidStatus,
                fakeValidGameStatus);

            // act
            GetGamesRequest request = new GetGamesRequest()
            {
                PlatformType = PlatformType.Desktop,
                CurrencyCode = "CNY",
                LanguageCode = "zh-TW",
                Types = Array.Empty<GameType>()
            };

            await sut.Handle(request, default);

            // assert
            // 驗證沒傳gpcode時 會執行一次 _brandOperations.GetBrandGameProvidersAsync 取得全部gameProviderId
            _ = fakeBrandOperations.Received(1).GetBrandGameProvidersAsync(Arg.Is(brandId));
        }

        [Test]
        public async Task Get_Game_Url_With_Extend_Data()
        {
            // arrange
            var playerCode = "playerCode";
            var fakeHttpContext = new DefaultHttpContext();
            var brandId = Guid.NewGuid();
            var gameProviderCode = "KY";
            var gameProviderId = new Guid("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5");
            var gameId = 1000012;
            var uri = new Uri("http://www.fakeurl.com");
            var extendedData = JsonDocument.Parse(@"{""lobbyurl"":""http://www.lobbyurl.com""}");
            var playerToken = new PlayerTokenData
            {
                PlayerCode = playerCode,
                ExpiredOn = new DateTimeOffset(new DateTime(2021, 5, 20)),
                LoginRecordId = Guid.NewGuid(),
                PlatformType = PlatformType.Desktop
            };
            fakeHttpContext.Items.Add(Consts.ContextItems.BrandId, brandId);
            fakeHttpContext.Items.Add("playerToken", playerToken);
            var fakeHttpContextAccessor = new HttpContextAccessor { HttpContext = fakeHttpContext };

            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandLanguageCodeMapper = Substitute.For<IBrandLanguageCodeMapper>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();
            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakeExpirationValidator = Substitute.For<IExpirationValidator>();
            var fakePluggalbeGameProviderApiClient = Substitute.For<IPluggalbeGameProviderApiClient>();
            var fakePluggableOperations = Substitute.For<IPluggableOperations>();

            fakeGameProviderOperations
                .GetGameProviderIdByCodeAsync(gameProviderCode)
                .Returns(gameProviderId);
            fakeGameProviderOperations
                .GetGamePlatformTypeAsync(gameProviderId, gameId)
                .Returns(GamePlatformType.Desktop);

            fakePlayerOperations
                .GetCachedPlayerAsync(playerCode)
                .Returns(new CachedPlayer()
                {
                    Status = PlayerStatus.Enabled
                });

            fakePluggableOperations
                .GetEndpointUrlByGameProviderCodeAsync(gameProviderCode)
                .Returns(uri);
            var req = new PluggableModels.GetGameUrlRequest()
            {
                ExtendedData = null,
                GameId = gameId,
                GameProviderCode = gameProviderCode,
                Language = null,
                Token = playerToken
            };
            fakePluggalbeGameProviderApiClient
                .GetGameUrlAsync(uri, Arg.Any<PluggableModels.GetGameUrlRequest>())
                .Returns(new PluggableModels.GetGameUrlResponse()
                {
                    Url = "http://www.lobbyURL.com",
                    Error = 0,
                    ErrorMessage = string.Empty
                });

            var sut = new GetGameUrlHandler(
                fakeHttpContextAccessor,
                fakeGameProviderOperations,
                fakePlayerOperations,
                fakeBrandLanguageCodeMapper,
                fakeValidStatus,
                fakeValidGameStatus,
                fakeStatusValidator,
                fakeExpirationValidator,
                fakePluggalbeGameProviderApiClient,
                fakePluggableOperations
                );

            // act
            var request = new GetGameUrlRequest()
            {
                GameId = gameId,
                GameProviderCode = gameProviderCode,
                LanguageCode = "zh-TW",
                ExtendedData = extendedData
            };

            await sut.Handle(request, default);

            // assert
            _ = fakePluggalbeGameProviderApiClient
                .Received(1)
                .GetGameUrlAsync(
                    uri,
                    Arg.Is<PluggableModels.GetGameUrlRequest>(
                        d => d.GameId == gameId
                            && d.GameProviderCode == gameProviderCode
                            && d.ExtendedData != null));
        }

        [Test]
        public async Task Get_Lobby_Game_Url_With_Extend_Data()
        {
            // arrange
            var playerCode = "playerCode";
            var fakeHttpContext = new DefaultHttpContext();
            var brandId = Guid.NewGuid();
            var gameType = GameType.CardGameLobby;
            var playerToken = new PlayerTokenData
            {
                PlayerCode = playerCode,
                ExpiredOn = new DateTimeOffset(new DateTime(2021, 5, 20)),
                LoginRecordId = Guid.NewGuid(),
                PlatformType = PlatformType.Desktop
            };
            fakeHttpContext.Items.Add(Consts.ContextItems.BrandId, brandId);
            fakeHttpContext.Items.Add("playerToken", playerToken);
            var fakeHttpContextAccessor = new HttpContextAccessor { HttpContext = fakeHttpContext };

            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakeMediator = Substitute.For<IMediator>();
            var gameProviderCode = "KY";
            var gameProviderId = new Guid("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5");
            var gameId = 1000012;
            var languageCode = "zh-TW";
            var extendedData = JsonDocument.Parse(@"{""lobbyurl"":""http://www.lobbyurl.com""}");

            fakeGameProviderOperations
                .GetGameProviderIdByCodeAsync(gameProviderCode)
                .Returns(gameProviderId);
            var externalGameId = "CardGameLobby";
            var cachedGameList = new List<CachedGame>
            {
                new CachedGame()
                {
                    Id = gameId,
                    ExternalId = externalGameId,
                    PlatformType = GamePlatformType.Desktop,
                    Type = gameType,
                    AvailableToBrand = true,
                    Status = GameStatus.Active,
                    SubGame = false,
                }
            };

            var cachedGames = cachedGameList.ToArray();
            fakeGameProviderOperations
                .GetCachedGamesAsync(gameProviderId, Arg.Any<GamePlatformType[]>(), gameType)
                .Returns(cachedGames);

            var sut = new GetLobbyGameUrlHandler(
                fakeHttpContextAccessor,
                fakeGameProviderOperations,
                fakeMediator
                );

            var lobbyRequest = new GetLobbyGameUrlRequest()
            {
                GameType = gameType,
                GameProviderCode = gameProviderCode,
                LanguageCode = languageCode,
                Extends = extendedData
            };
            fakeMediator
                .Send(Arg.Any<GetGameUrlRequest>(), Arg.Any<CancellationToken>())
                .Returns(new GetGameUrlResponse()
                {
                    Error = 0,
                    ErrorMessage = string.Empty,
                    Url = "http://www.lobbyURL.com"
                });

            // act
            var actual = await sut.Handle(lobbyRequest, default);

            // assert
            Assert.AreEqual("http://www.lobbyURL.com", actual.Url);
            Assert.AreEqual(0, actual.Error);
        }
    }
}