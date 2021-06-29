using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Game;
using TGS.Services.PluggableServices;
using TGS.ServiceUtility;
using PluggableModels = TGS.Services.PluggableServices.Models;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public async Task Get_Game_List_Of_All_GameProviders()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var gameProviderCode = string.Empty;

            var query = new GetGamesQuery(
                brandId,
                CurrencyCode: "CNY",
                gameProviderCode,
                new PlayerTokenData(),
                PlatformType.Desktop,
                Types: Array.Empty<GameType>(),
                LanguageCode: "zh-CN");

            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandLanguageCodeMapper = Substitute.For<IBrandLanguageCodeMapper>();
            var fakeLocalizationOperations = Substitute.For<ILocalizationOperations>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();

            var sut = new GetGamesQueryService(
                fakeBrandCurrencyCodeMapper,
                fakeGameProviderOperations,
                fakeBrandOperations,
                fakePlayerOperations,
                fakeBrandLanguageCodeMapper,
                fakeLocalizationOperations,
                fakeValidStatus,
                fakeValidGameStatus);

            // act
            await sut.QueryAsync(query, default).ToArrayAsync();

            // assert
            // 驗證沒傳gpcode時 會執行一次 _brandOperations.GetBrandGameProvidersAsync 取得全部gameProviderId
            _ = fakeBrandOperations.Received(1).GetBrandGameProvidersAsync(Arg.Is(brandId));
        }

        [Test]
        public async Task Get_Game_List_Of_Specific_GameProvider()
        {
            // arrange
            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandLanguageCodeMapper = Substitute.For<IBrandLanguageCodeMapper>();
            var fakeLocalizationOperations = Substitute.For<ILocalizationOperations>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();

            var sut = new GetGamesQueryService(
                fakeBrandCurrencyCodeMapper,
                fakeGameProviderOperations,
                fakeBrandOperations,
                fakePlayerOperations,
                fakeBrandLanguageCodeMapper,
                fakeLocalizationOperations,
                fakeValidStatus,
                fakeValidGameStatus);

            var gameProviderCode = "SBO";

            var query = new GetGamesQuery(
                BrandId: Guid.NewGuid(),
                CurrencyCode: "CNY",
                gameProviderCode,
                new PlayerTokenData(),
                PlatformType.Desktop,
                Types: Array.Empty<GameType>(),
                LanguageCode: "zh-CN");

            // act
            await sut.QueryAsync(query, default).ToArrayAsync();

            // assert
            // 驗證有傳gpcode時 會執行一次 _gameProviderOperations.GetGameProviderIdByCodeAsync 取得gameProviderId
            _ = fakeGameProviderOperations.Received(1).GetGameProviderIdByCodeAsync(Arg.Is(gameProviderCode));
        }

        [Test]
        public async Task Get_Game_Url_With_Extend_Data()
        {
            // arrange
            var playerCode = "playerCode";
            var languageCode = "zh-CN";
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

            var query = new GetGameUrlQuery(
               playerToken,
               gameProviderCode,
               gameId,
               languageCode,
               extendedData);

            var sut = new GetGameUrlQueryService(
                fakePlayerOperations,
                fakeStatusValidator,
                fakeExpirationValidator,
                fakeGameProviderOperations,
                fakeValidStatus,
                fakeValidGameStatus,
                fakeBrandLanguageCodeMapper,
                fakePluggableOperations,
                fakePluggalbeGameProviderApiClient);

            // act
            await sut.GetAsync(query, default);

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
            var gameType = GameType.CardGameLobby;
            var playerToken = new PlayerTokenData
            {
                PlayerCode = playerCode,
                ExpiredOn = new DateTimeOffset(new DateTime(2021, 5, 20)),
                LoginRecordId = Guid.NewGuid(),
                PlatformType = PlatformType.Desktop
            };

            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakeGetGameUrlQueryService = Substitute.For<IGetService<GetGameUrlQuery, GameUrlInfo>>();

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

            fakeGetGameUrlQueryService
                .GetAsync(Arg.Any<GetGameUrlQuery>(), default)
                .Returns(new GameUrlInfo(0, "", "http://www.lobbyURL.com"));

            var query = new GetLobbyGameUrlQuery(
                gameProviderCode,
                playerToken,
                gameType,
                languageCode,
                extendedData);

            var sut = new GetLobbyGameUrlQueryService(
                fakeGameProviderOperations,
                fakeGetGameUrlQueryService);
            // act
            var actual = await sut.GetAsync(query, default);

            // assert
            Assert.AreEqual("http://www.lobbyURL.com", actual.Url);
            Assert.AreEqual(0, actual.Error);
        }
    }
}