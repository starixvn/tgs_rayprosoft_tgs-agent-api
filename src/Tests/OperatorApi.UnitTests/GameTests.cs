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
using TGS.Services.Operator.GameProviders;
using TGS.Services.Operator.Games;
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
            var currencyCode = "CNY";
            var gameProviderCode = string.Empty;

            var gameProviders = new GameProviderDataInfo[]
            {
                new(Guid.NewGuid(), "EVO", Status.Active, false),
                new(Guid.NewGuid(), "SBO", Status.Active, false)
            };

            var subGameProviders = GetSubGameProviders(gameProviders).ToArray();
            var games = GetGames(subGameProviders).ToArray();

            var brandGameProviders = gameProviders.Select(gp => new CachedBrandGameProvider
            {
                GameProviderId = gp.Id,
                GameProviderCode = gp.Code,
                GameProviderConfigurationId = Guid.NewGuid(),
                BrandGameProviderStatus = Status.Active
            }).ToArray();

            var brandGames = games.Select(g => new CachedBrandGame
            {
                GameId = g.Id,
                GameProviderId = subGameProviders.Where(sbp => sbp.Code == g.SubGameProviderCode)
                    .Select(sbp => sbp.GameProviderId).First(),
                PlatformType = g.PlatformType,
                BrandGameStatus = g.Status,
                Tag = GameTag.None,
                DisplayOrder = 99,
                ExternalGameId = g.Id.ToString()
            }).ToArray();

            var currencies = gameProviders.Select(gp => new GameProviderCurrencyDataInfo(gp.Id, currencyCode))
                .ToArray();

            var query = new GetGamesQuery(
                brandId,
                currencyCode,
                gameProviderCode,
                new PlayerTokenData(),
                PlatformType.Desktop,
                Array.Empty<GameType>(),
                "zh-CN");

            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandLanguageCodeMapper = Substitute.For<IBrandLanguageCodeMapper>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameRepository = Substitute.For<IGameRepository>();

            fakeBrandOperations.GetBrandGameProvidersAsync(query.BrandId)
                .Returns(Task.FromResult(brandGameProviders));

            fakeBrandOperations.GetBrandGamesAsync(query.BrandId)
                .Returns(Task.FromResult(brandGames));

            fakeGameProviderRepository.QuerySubGameProvidersAsync(
                    Arg.Any<string>())
                .Returns(subGameProviders.ToAsyncEnumerable());

            fakeGameProviderRepository.QueryGameProvidersAsync(
                    Arg.Any<IEnumerable<Guid>>(),
                    Arg.Any<IEnumerable<Status>>())
                .Returns(gameProviders.ToAsyncEnumerable());

            fakeGameProviderRepository.QueryGameProviderCurrenciesAsync(Arg.Any<IEnumerable<Guid>>())
                .Returns(currencies.ToAsyncEnumerable());

            fakeGameRepository.QueryGamesAsync(
                    Arg.Any<IEnumerable<Guid>>(),
                    Arg.Any<IEnumerable<GameStatus>>(),
                    Arg.Any<IEnumerable<GamePlatformType>>())
                .Returns(games.ToAsyncEnumerable());

            var sut = new GetGamesQueryService(
                fakeBrandCurrencyCodeMapper,
                fakeBrandOperations,
                fakePlayerOperations,
                fakeBrandLanguageCodeMapper,
                fakeValidStatus,
                fakeValidGameStatus,
                fakeGameProviderRepository,
                fakeGameRepository);

            // act
            var result = await sut.QueryAsync(query).ToArrayAsync();

            // assert
            Assert.AreEqual(games.Length, result.Length);

            static IEnumerable<SubGameProviderDataInfo> GetSubGameProviders(
                IEnumerable<GameProviderDataInfo> gameProviders)
            {
                foreach (var gp in gameProviders)
                {
                    if (gp.Code == "EVO")
                    {
                        yield return new SubGameProviderDataInfo(gp.Id, "EVO");
                        yield return new SubGameProviderDataInfo(gp.Id, "NT");
                        yield return new SubGameProviderDataInfo(gp.Id, "RT");
                    }
                    else
                    {
                        yield return new SubGameProviderDataInfo(gp.Id, gp.Code);
                    }
                }
            }

            static IEnumerable<GameDataInfo> GetGames(IEnumerable<SubGameProviderDataInfo> subGameProviders)
            {
                var gameId = 0;
                foreach (var gp in subGameProviders)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        gameId++;

                        var gameType = gp.Code switch
                        {
                            "SBO" => GameType.Sportsbook,
                            "EVO" => GameType.LiveDealer,
                            _     => GameType.Slot
                        };

                        yield return new GameDataInfo(
                            gameId,
                            gameId.ToString(),
                            gameId.ToString(),
                            string.Empty,
                            gp.Code,
                            GameStatus.Active,
                            gameType,
                            GamePlatformType.DesktopAndMobile,
                            true,
                            string.Empty,
                            string.Empty);
                    }
                }
            }
        }

        [Test]
        public async Task Get_Game_List_Of_Specific_GameProvider()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var currencyCode = "CNY";
            var gameProviderCode = "RT";

            var gameProviders = new GameProviderDataInfo[]
            {
                new(Guid.NewGuid(), "EVO", Status.Active, false),
                new(Guid.NewGuid(), "SBO", Status.Active, false)
            };

            var subGameProviders = GetSubGameProviders(gameProviders).ToArray();
            var games = GetGames(subGameProviders).ToArray();

            var brandGameProviders = gameProviders.Select(gp => new CachedBrandGameProvider
            {
                GameProviderId = gp.Id,
                GameProviderCode = gp.Code,
                GameProviderConfigurationId = Guid.NewGuid(),
                BrandGameProviderStatus = Status.Active
            }).ToArray();

            var brandGames = games.Select(g => new CachedBrandGame
            {
                GameId = g.Id,
                GameProviderId = subGameProviders.Where(sbp => sbp.Code == g.SubGameProviderCode)
                    .Select(sbp => sbp.GameProviderId).First(),
                PlatformType = g.PlatformType,
                BrandGameStatus = g.Status,
                Tag = GameTag.None,
                DisplayOrder = 99,
                ExternalGameId = g.Id.ToString()
            }).ToArray();

            var currencies = gameProviders.Select(gp => new GameProviderCurrencyDataInfo(gp.Id, currencyCode))
                .ToArray();

            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandLanguageCodeMapper = Substitute.For<IBrandLanguageCodeMapper>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameRepository = Substitute.For<IGameRepository>();

            var query = new GetGamesQuery(
                brandId,
                currencyCode,
                gameProviderCode,
                new PlayerTokenData(),
                PlatformType.Desktop,
                Array.Empty<GameType>(),
                "zh-CN");

            fakeBrandOperations.GetBrandGameProvidersAsync(query.BrandId)
                .Returns(Task.FromResult(brandGameProviders));

            fakeBrandOperations.GetBrandGamesAsync(query.BrandId)
                .Returns(Task.FromResult(brandGames));

            fakeGameProviderRepository.QuerySubGameProvidersAsync(
                    Arg.Any<string>())
                .Returns(subGameProviders.ToAsyncEnumerable());

            fakeGameProviderRepository.QueryGameProvidersAsync(
                    Arg.Any<IEnumerable<Guid>>(),
                    Arg.Any<IEnumerable<Status>>())
                .Returns(gameProviders.ToAsyncEnumerable());

            fakeGameProviderRepository.QueryGameProviderCurrenciesAsync(Arg.Any<IEnumerable<Guid>>())
                .Returns(currencies.ToAsyncEnumerable());

            fakeGameRepository.QueryGamesAsync(
                    Arg.Any<IEnumerable<Guid>>(),
                    Arg.Any<IEnumerable<GameStatus>>(),
                    Arg.Any<IEnumerable<GamePlatformType>>())
                .Returns(games.ToAsyncEnumerable());

            var sut = new GetGamesQueryService(
                fakeBrandCurrencyCodeMapper,
                fakeBrandOperations,
                fakePlayerOperations,
                fakeBrandLanguageCodeMapper,
                fakeValidStatus,
                fakeValidGameStatus,
                fakeGameProviderRepository,
                fakeGameRepository);

            // act
            var result = await sut.QueryAsync(query).ToArrayAsync();

            // assert
            foreach (var r in result)
            {
                Assert.AreEqual(gameProviderCode, r.GameProviderCode);
            }

            static IEnumerable<SubGameProviderDataInfo> GetSubGameProviders(
                IEnumerable<GameProviderDataInfo> gameProviders)
            {
                foreach (var gp in gameProviders)
                {
                    if (gp.Code == "EVO")
                    {
                        yield return new SubGameProviderDataInfo(gp.Id, "EVO");
                        yield return new SubGameProviderDataInfo(gp.Id, "NT");
                        yield return new SubGameProviderDataInfo(gp.Id, "RT");
                    }
                    else
                    {
                        yield return new SubGameProviderDataInfo(gp.Id, gp.Code);
                    }
                }
            }

            static IEnumerable<GameDataInfo> GetGames(IEnumerable<SubGameProviderDataInfo> subGameProviders)
            {
                var gameId = 0;
                foreach (var gp in subGameProviders)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        gameId++;

                        var gameType = gp.Code switch
                        {
                            "SBO" => GameType.Sportsbook,
                            "EVO" => GameType.LiveDealer,
                            _     => GameType.Slot
                        };

                        yield return new GameDataInfo(
                            gameId,
                            gameId.ToString(),
                            gameId.ToString(),
                            string.Empty,
                            gp.Code,
                            GameStatus.Active,
                            gameType,
                            GamePlatformType.DesktopAndMobile,
                            true,
                            string.Empty,
                            string.Empty);
                    }
                }
            }
        }

        [Test]
        public async Task Get_Game_List_Of_Specific_GameTypes()
        {
            // arrange
            var brandId = Guid.NewGuid();
            var currencyCode = "CNY";
            var gameProviderCode = string.Empty;

            var gameProviders = new GameProviderDataInfo[]
            {
                new(Guid.NewGuid(), "EVO", Status.Active, false),
                new(Guid.NewGuid(), "SBO", Status.Active, false)
            };

            var subGameProviders = GetSubGameProviders(gameProviders).ToArray();
            var games = GetGames(subGameProviders).ToArray();

            var brandGameProviders = gameProviders.Select(gp => new CachedBrandGameProvider
            {
                GameProviderId = gp.Id,
                GameProviderCode = gp.Code,
                GameProviderConfigurationId = Guid.NewGuid(),
                BrandGameProviderStatus = Status.Active
            }).ToArray();

            var brandGames = games.Select(g => new CachedBrandGame
            {
                GameId = g.Id,
                GameProviderId = subGameProviders.Where(sbp => sbp.Code == g.SubGameProviderCode)
                    .Select(sbp => sbp.GameProviderId).First(),
                PlatformType = g.PlatformType,
                BrandGameStatus = g.Status,
                Tag = GameTag.None,
                DisplayOrder = 99,
                ExternalGameId = g.Id.ToString()
            }).ToArray();

            var currencies = gameProviders.Select(gp => new GameProviderCurrencyDataInfo(gp.Id, currencyCode))
                .ToArray();

            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandLanguageCodeMapper = Substitute.For<IBrandLanguageCodeMapper>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameRepository = Substitute.For<IGameRepository>();

            var gameTypes = new[]
            {
                GameType.Sportsbook,
                GameType.Slot
            };

            var query = new GetGamesQuery(
                brandId,
                currencyCode,
                gameProviderCode,
                new PlayerTokenData(),
                PlatformType.Desktop,
                gameTypes,
                "zh-CN");

            fakeBrandOperations.GetBrandGameProvidersAsync(query.BrandId)
                .Returns(Task.FromResult(brandGameProviders));

            fakeBrandOperations.GetBrandGamesAsync(query.BrandId)
                .Returns(Task.FromResult(brandGames));

            fakeGameProviderRepository.QuerySubGameProvidersAsync(
                    Arg.Any<string>())
                .Returns(subGameProviders.ToAsyncEnumerable());

            fakeGameProviderRepository.QueryGameProvidersAsync(
                    Arg.Any<IEnumerable<Guid>>(),
                    Arg.Any<IEnumerable<Status>>())
                .Returns(gameProviders.ToAsyncEnumerable());

            fakeGameProviderRepository.QueryGameProviderCurrenciesAsync(Arg.Any<IEnumerable<Guid>>())
                .Returns(currencies.ToAsyncEnumerable());

            fakeGameRepository.QueryGamesAsync(
                    Arg.Any<IEnumerable<Guid>>(),
                    Arg.Any<IEnumerable<GameStatus>>(),
                    Arg.Any<IEnumerable<GamePlatformType>>())
                .Returns(games.ToAsyncEnumerable());

            var sut = new GetGamesQueryService(
                fakeBrandCurrencyCodeMapper,
                fakeBrandOperations,
                fakePlayerOperations,
                fakeBrandLanguageCodeMapper,
                fakeValidStatus,
                fakeValidGameStatus,
                fakeGameProviderRepository,
                fakeGameRepository);

            // act
            var result = await sut.QueryAsync(query).ToArrayAsync();

            // assert
            foreach (var r in result)
            {
                Assert.That(gameTypes.Contains(r.Type));
            }

            static IEnumerable<SubGameProviderDataInfo> GetSubGameProviders(
                IEnumerable<GameProviderDataInfo> gameProviders)
            {
                foreach (var gp in gameProviders)
                {
                    if (gp.Code == "EVO")
                    {
                        yield return new SubGameProviderDataInfo(gp.Id, "EVO");
                        yield return new SubGameProviderDataInfo(gp.Id, "NT");
                        yield return new SubGameProviderDataInfo(gp.Id, "RT");
                    }
                    else
                    {
                        yield return new SubGameProviderDataInfo(gp.Id, gp.Code);
                    }
                }
            }

            static IEnumerable<GameDataInfo> GetGames(IEnumerable<SubGameProviderDataInfo> subGameProviders)
            {
                var gameId = 0;
                foreach (var gp in subGameProviders)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        gameId++;

                        var gameType = gp.Code switch
                        {
                            "SBO" => GameType.Sportsbook,
                            "EVO" => GameType.LiveDealer,
                            _     => GameType.Slot
                        };

                        yield return new GameDataInfo(
                            gameId,
                            gameId.ToString(),
                            gameId.ToString(),
                            string.Empty,
                            gp.Code,
                            GameStatus.Active,
                            gameType,
                            GamePlatformType.DesktopAndMobile,
                            true,
                            string.Empty,
                            string.Empty);
                    }
                }
            }
        }

        [Test]
        public async Task Get_Game_Url_With_Extend_Data()
        {
            // arrange
            var playerCode = "playerCode";
            var languageCode = "zh-CN";
            var subGameProviderCode = "RT";
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

            var subGameProviders = new SubGameProviderDataInfo[]
            {
                new(gameProviderId, "RT")
            };

            var gameProvider = new CachedGameProvider
            {
                Id = gameProviderId,
                Code = "EVO",
                Status = Status.Active
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
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

            fakeGameProviderRepository.QuerySubGameProvidersAsync(subGameProviderCode)
                .Returns(subGameProviders.ToAsyncEnumerable());

            fakeGameProviderOperations
                .GetCachedGameProviderAsync(gameProviderId)
                .Returns(Task.FromResult(gameProvider));

            fakeGameProviderOperations
                .GetGamePlatformTypeAsync(gameProviderId, gameId)
                .Returns(GamePlatformType.Desktop);

            fakePlayerOperations
                .GetCachedPlayerAsync(playerCode)
                .Returns(new CachedPlayer {Status = PlayerStatus.Enabled});

            fakePluggableOperations
                .GetEndpointUrlByGameProviderCodeAsync(gameProvider.Code)
                .Returns(Task.FromResult(uri));

            fakePluggalbeGameProviderApiClient
                .GetGameUrlAsync(Arg.Is(uri), Arg.Any<PluggableModels.GetGameUrlRequest>())
                .Returns(
                    ValueTask.FromResult(
                        new PluggableModels.GetGameUrlResponse
                        {
                            Url = "http://www.lobbyURL.com",
                            Error = 0,
                            ErrorMessage = string.Empty
                        }));

            var query = new GetGameUrlQuery(
                playerToken,
                subGameProviderCode,
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
                fakePluggalbeGameProviderApiClient,
                fakeGameProviderRepository);

            // act
            await sut.GetAsync(query);

            // assert
            _ = fakePluggalbeGameProviderApiClient
                .Received(1)
                .GetGameUrlAsync(
                    uri,
                    Arg.Is<PluggableModels.GetGameUrlRequest>(
                        d => d.GameId == gameId
                            && d.GameProviderCode == gameProvider.Code
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
                new CachedGame
                {
                    Id = gameId,
                    ExternalId = externalGameId,
                    PlatformType = GamePlatformType.Desktop,
                    Type = gameType,
                    AvailableToBrand = true,
                    Status = GameStatus.Active,
                    SubGame = false
                }
            };

            var cachedGames = cachedGameList.ToArray();
            fakeGameProviderOperations
                .GetCachedGamesAsync(gameProviderId, Arg.Any<GamePlatformType[]>(), gameType)
                .Returns(cachedGames);

            fakeGetGameUrlQueryService
                .GetAsync(Arg.Any<GetGameUrlQuery>())
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
            var actual = await sut.GetAsync(query);

            // assert
            Assert.AreEqual("http://www.lobbyURL.com", actual.Url);
            Assert.AreEqual(0, actual.Error);
        }
    }
}