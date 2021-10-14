using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Controllers;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;
using TGS.Services.Operator;
using TGS.Services.Operator.GameProviders;
using TGS.Services.Operator.Services.Game;
using TGS.Services.Operator.Services.GameBalance;
using TGS.Services.Operator.Services.Wallet;
using TGS.Services.PluggableServices;
using TGS.ServiceUtility;
using PluggableModels = TGS.Services.PluggableServices.Models;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public async Task Enter_Game_If_Flag_Is_False_Not_FundOut_Wallet_Amount()
        {
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeGetPlayerBalanceService = Substitute.For<IGetService<GetBalanceQuery, BalanceInfo>>();
            var fakeFundOutService = Substitute.For<ICommandService<FundOutGameBalanceCommand>>();

            var sut = new FundOutBeforeEnterGameCommandService(
                fakePlayerRepository,
                fakeGetPlayerBalanceService,
                fakeFundOutService);

            var command = new FundOutBeforeEnterGameCommand(
                Guid.NewGuid(),
                "ThisIsPlayerCode",
                "gp1");

            fakePlayerRepository.GetPlayerAutoTransferFlagAsync(Arg.Is(command.PlayerCode))
                .Returns(false);

            fakeGetPlayerBalanceService.GetAsync(Arg.Any<GetBalanceQuery>())
                .Returns(new BalanceInfo(1000M));

            await sut.ExecuteAsync(command);

            _ = fakeFundOutService.Received(0)
                .ExecuteAsync(Arg.Any<FundOutGameBalanceCommand>());
        }

        [Test]
        public async Task Enter_Game_If_Flag_Is_True_Should_FundOut_Wallet_Amount()
        {
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeGetPlayerBalanceService = Substitute.For<IGetService<GetBalanceQuery, BalanceInfo>>();
            var fakeFundOutService = Substitute.For<ICommandService<FundOutGameBalanceCommand>>();

            var sut = new FundOutBeforeEnterGameCommandService(
                fakePlayerRepository,
                fakeGetPlayerBalanceService,
                fakeFundOutService);

            var command = new FundOutBeforeEnterGameCommand(
                Guid.NewGuid(),
                "ThisIsPlayerCode",
                "gp1");

            fakePlayerRepository.GetPlayerAutoTransferFlagAsync(Arg.Is(command.PlayerCode))
                .Returns(true);

            fakeGetPlayerBalanceService.GetAsync(Arg.Any<GetBalanceQuery>())
                .Returns(new BalanceInfo(1000M));

            await sut.ExecuteAsync(command);

            _ = fakeFundOutService.Received(1)
                .ExecuteAsync(Arg.Is<FundOutGameBalanceCommand>(
                    cmd => cmd.PlayerCode == command.PlayerCode
                        && cmd.Amount == 1000));
        }

        [Test]
        public async Task FundInAllGameBalance_Should_Call_All_GP_FundIn()
        {
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGetGameBalanceService = Substitute.For<IGetService<GetGameBalanceQuery, GameBalanceInfo>>();
            var fakeGameFundInService = Substitute.For<ICommandService<FundInGameBalanceCommand>>();

            var sut = new FundInAllGameBalanceCommandService(
                fakeGameProviderRepository,
                fakeGetGameBalanceService,
                fakeGameFundInService);

            var command = new FundInAllGameBalanceCommand(
                Guid.NewGuid(),
                "ThisIsPlayerCode",
                "GpCode");

            fakeGameProviderRepository.QueryAllSubGameProvidersAsync(Arg.Is(command.BrandId))
                .Returns(new[]
                {
                    new BrandSubGameProviderInfo(
                        "sub1",
                        "sub1",
                        "sub1",
                        Guid.NewGuid(),
                        "gp1",
                        true,
                        Status.Active,
                        Status.Active),
                    new BrandSubGameProviderInfo(
                        "sub2",
                        "sub2",
                        "sub2",
                        Guid.NewGuid(),
                        "gp1",
                        true,
                        Status.Active,
                        Status.Active),
                    new BrandSubGameProviderInfo(
                        "sub3",
                        "sub3",
                        "sub3",
                        Guid.NewGuid(),
                        "gp2",
                        true,
                        Status.Active,
                        Status.Active)
                }.ToAsyncEnumerable());

            fakeGetGameBalanceService.GetAsync(Arg.Any<GetGameBalanceQuery>())
                .Returns(new GameBalanceInfo
                {
                    Balance = 100M,
                    Error = 0
                });

            await sut.ExecuteAsync(command);

            _ = fakeGameFundInService.Received(2)
                .ExecuteAsync(Arg.Any<FundInGameBalanceCommand>());
        }

        [Test]
        public async Task FundInAllGameBalance_Should_Call_GP_FundIn_When_Amount_Equals_One_Dollar()
        {
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGetGameBalanceService = Substitute.For<IGetService<GetGameBalanceQuery, GameBalanceInfo>>();
            var fakeGameFundInService = Substitute.For<ICommandService<FundInGameBalanceCommand>>();

            var sut = new FundInAllGameBalanceCommandService(
                fakeGameProviderRepository,
                fakeGetGameBalanceService,
                fakeGameFundInService);

            var command = new FundInAllGameBalanceCommand(
                Guid.NewGuid(),
                "ThisIsPlayerCode",
                "GpCode");

            fakeGameProviderRepository.QueryAllSubGameProvidersAsync(Arg.Is(command.BrandId))
                .Returns(new[]
                {
                    new BrandSubGameProviderInfo(
                        "sub1",
                        "sub1",
                        "sub1",
                        Guid.NewGuid(),
                        "gp1",
                        true,
                        Status.Active,
                        Status.Active),
                    new BrandSubGameProviderInfo(
                        "sub2",
                        "sub2",
                        "sub2",
                        Guid.NewGuid(),
                        "gp1",
                        true,
                        Status.Active,
                        Status.Active),
                    new BrandSubGameProviderInfo(
                        "sub3",
                        "sub3",
                        "sub3",
                        Guid.NewGuid(),
                        "gp2",
                        true,
                        Status.Active,
                        Status.Active)
                }.ToAsyncEnumerable());

            fakeGetGameBalanceService.GetAsync(Arg.Any<GetGameBalanceQuery>())
                .Returns(new GameBalanceInfo
                {
                    Balance = 1m,
                    Error = 0
                });

            await sut.ExecuteAsync(command);

            _ = fakeGameFundInService.Received(2)
                .ExecuteAsync(Arg.Any<FundInGameBalanceCommand>());
        }

        [Test]
        public async Task FundInAllGameBalance_Should_Not_Call_GP_FundIn_When_Amount_LessThan_One_Dollar()
        {
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGetGameBalanceService = Substitute.For<IGetService<GetGameBalanceQuery, GameBalanceInfo>>();
            var fakeGameFundInService = Substitute.For<ICommandService<FundInGameBalanceCommand>>();

            var sut = new FundInAllGameBalanceCommandService(
                fakeGameProviderRepository,
                fakeGetGameBalanceService,
                fakeGameFundInService);

            var command = new FundInAllGameBalanceCommand(
                Guid.NewGuid(),
                "ThisIsPlayerCode",
                "GpCode");

            fakeGameProviderRepository.QueryAllSubGameProvidersAsync(Arg.Is(command.BrandId))
                .Returns(new[]
                {
                    new BrandSubGameProviderInfo(
                        "sub1",
                        "sub1",
                        "sub1",
                        Guid.NewGuid(),
                        "gp1",
                        true,
                        Status.Active,
                        Status.Active),
                    new BrandSubGameProviderInfo(
                        "sub2",
                        "sub2",
                        "sub2",
                        Guid.NewGuid(),
                        "gp1",
                        true,
                        Status.Active,
                        Status.Active),
                    new BrandSubGameProviderInfo(
                        "sub3",
                        "sub3",
                        "sub3",
                        Guid.NewGuid(),
                        "gp2",
                        true,
                        Status.Active,
                        Status.Active)
                }.ToAsyncEnumerable());

            fakeGetGameBalanceService.GetAsync(Arg.Any<GetGameBalanceQuery>())
                .Returns(new GameBalanceInfo
                {
                    Balance = 0.88m,
                    Error = 0
                });

            await sut.ExecuteAsync(command);

            _ = fakeGameFundInService.Received(0)
                .ExecuteAsync(Arg.Any<FundInGameBalanceCommand>());
        }

        [Test]
        public async Task Get_Game_List_Of_All_GameProviders()
        {
            // arrange
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

            var brandId = Guid.NewGuid();

            var evoGp = new GameProviderDataInfo(Guid.NewGuid(), "EVO", Status.Active, false);
            var sboGp = new GameProviderDataInfo(Guid.NewGuid(), "SBO", Status.Active, false);

            var subGameProviders = new[]
            {
                new BrandSubGameProviderInfo(
                    "EVO",
                    "EVO",
                    "EVO",
                    evoGp.Id,
                    evoGp.Code,
                    false,
                    Status.Active,
                    Status.Active),
                new BrandSubGameProviderInfo(
                    "NT",
                    "NT",
                    "NT",
                    evoGp.Id,
                    evoGp.Code,
                    false,
                    Status.Active,
                    Status.Active),
                new BrandSubGameProviderInfo(
                    "RT",
                    "RT",
                    "RT",
                    evoGp.Id,
                    evoGp.Code,
                    false,
                    Status.Active,
                    Status.Active),
                new BrandSubGameProviderInfo(
                    "SBO",
                    "SBO",
                    "SBO",
                    sboGp.Id,
                    sboGp.Code,
                    false,
                    Status.Active,
                    Status.Active)
            };

            var games = new[]
            {
                new BrandGameInfo(
                    1,
                    "1",
                    "1",
                    "test",
                    "EVO",
                    evoGp.Id,
                    "EVO",
                    GameStatus.Active,
                    GameStatus.Active,
                    GameType.LiveDealer,
                    GamePlatformType.Desktop,
                    true,
                    string.Empty,
                    string.Empty,
                    GameTag.New,
                    0),
                new BrandGameInfo(
                    2,
                    "2",
                    "2",
                    "test",
                    "RT",
                    evoGp.Id,
                    "EVO",
                    GameStatus.Active,
                    GameStatus.Active,
                    GameType.Slot,
                    GamePlatformType.Desktop,
                    true,
                    string.Empty,
                    string.Empty,
                    GameTag.New,
                    0),
                new BrandGameInfo(
                    3,
                    "3",
                    "3",
                    "test",
                    "NT",
                    evoGp.Id,
                    "EVO",
                    GameStatus.Active,
                    GameStatus.Active,
                    GameType.Slot,
                    GamePlatformType.Desktop,
                    true,
                    string.Empty,
                    string.Empty,
                    GameTag.New,
                    0),
                new BrandGameInfo(
                    4,
                    "4",
                    "4",
                    "test",
                    "SBO",
                    sboGp.Id,
                    "SBO",
                    GameStatus.Active,
                    GameStatus.Active,
                    GameType.Sportsbook,
                    GamePlatformType.Desktop,
                    true,
                    string.Empty,
                    string.Empty,
                    GameTag.New,
                    0)
            };

            fakeValidStatus.GetStatus(Arg.Any<PlayerType>())
                .Returns(new[]
                {
                    Status.Active,
                    Status.Maintenance
                });

            fakeValidGameStatus.GetStatus(Arg.Any<PlayerType>())
                .Returns(new[]
                {
                    GameStatus.Active,
                    GameStatus.Maintenance,
                    GameStatus.Testing
                });

            fakeGameProviderRepository.QueryAllSubGameProvidersAsync(Arg.Is(brandId))
                .Returns(subGameProviders.ToAsyncEnumerable());

            fakeGameProviderRepository.QueryAllGamesAsync(Arg.Is(brandId))
                .Returns(games.ToAsyncEnumerable());

            var query = new GetAllGamesByGameTypeQuery(
                brandId,
                PlayerType.RealPlayer,
                PlatformType.Desktop,
                Array.Empty<GameType>());

            var sut = new GetAllGamesByGameTypeQueryService(
                fakeValidStatus,
                fakeValidGameStatus,
                fakeGameProviderRepository);

            // act
            var result = await sut.QueryAsync(query).ToArrayAsync();

            // assert
            Assert.AreEqual(games.Length, result.Length);
        }

        [Test]
        public async Task Get_Game_List_Of_Specific_GameProvider()
        {
            // arrange
            var fakeQueryAllService = Substitute.For<IQueryService<GetAllGamesByGameTypeQuery, GameInfo>>();

            var sut = new GetGamesQueryService(fakeQueryAllService);

            var query = new GetGamesQuery(
                Guid.NewGuid(),
                "Test",
                PlayerType.RealPlayer,
                PlatformType.Desktop,
                Array.Empty<GameType>());

            fakeQueryAllService.QueryAsync(Arg.Any<GetAllGamesByGameTypeQuery>())
                .Returns(new[]
                {
                    new GameInfo
                    {
                        Id = 1,
                        Code = "test",
                        Name = "test",
                        GameProviderCode = "Test"
                    },
                    new GameInfo
                    {
                        Id = 2,
                        Code = "test",
                        Name = "test",
                        GameProviderCode = "ABC"
                    }
                }.ToAsyncEnumerable());

            // act
            var result = await sut.QueryAsync(query).ToArrayAsync();

            // assert
            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public async Task Get_Game_List_Of_Specific_GameTypes()
        {
            // arrange
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

            var brandId = Guid.NewGuid();

            var evoGp = new GameProviderDataInfo(Guid.NewGuid(), "EVO", Status.Active, false);
            var sboGp = new GameProviderDataInfo(Guid.NewGuid(), "SBO", Status.Active, false);

            var subGameProviders = new[]
            {
                new BrandSubGameProviderInfo(
                    "EVO",
                    "EVO",
                    "EVO",
                    evoGp.Id,
                    evoGp.Code,
                    false,
                    Status.Active,
                    Status.Active),
                new BrandSubGameProviderInfo(
                    "NT",
                    "NT",
                    "NT",
                    evoGp.Id,
                    evoGp.Code,
                    false,
                    Status.Active,
                    Status.Active),
                new BrandSubGameProviderInfo(
                    "RT",
                    "RT",
                    "RT",
                    evoGp.Id,
                    evoGp.Code,
                    false,
                    Status.Active,
                    Status.Active),
                new BrandSubGameProviderInfo(
                    "SBO",
                    "SBO",
                    "SBO",
                    sboGp.Id,
                    sboGp.Code,
                    false,
                    Status.Active,
                    Status.Active)
            };

            var games = new[]
            {
                new BrandGameInfo(
                    1,
                    "1",
                    "1",
                    "test",
                    "EVO",
                    evoGp.Id,
                    "EVO",
                    GameStatus.Active,
                    GameStatus.Active,
                    GameType.LiveDealer,
                    GamePlatformType.Desktop,
                    true,
                    string.Empty,
                    string.Empty,
                    GameTag.New,
                    0),
                new BrandGameInfo(
                    2,
                    "2",
                    "2",
                    "test",
                    "RT",
                    evoGp.Id,
                    "EVO",
                    GameStatus.Active,
                    GameStatus.Active,
                    GameType.Slot,
                    GamePlatformType.Desktop,
                    true,
                    string.Empty,
                    string.Empty,
                    GameTag.New,
                    0),
                new BrandGameInfo(
                    3,
                    "3",
                    "3",
                    "test",
                    "NT",
                    evoGp.Id,
                    "EVO",
                    GameStatus.Active,
                    GameStatus.Active,
                    GameType.Slot,
                    GamePlatformType.Desktop,
                    true,
                    string.Empty,
                    string.Empty,
                    GameTag.New,
                    0),
                new BrandGameInfo(
                    4,
                    "4",
                    "4",
                    "test",
                    "SBO",
                    sboGp.Id,
                    "SBO",
                    GameStatus.Active,
                    GameStatus.Active,
                    GameType.Sportsbook,
                    GamePlatformType.Desktop,
                    true,
                    string.Empty,
                    string.Empty,
                    GameTag.New,
                    0)
            };

            fakeValidStatus.GetStatus(Arg.Any<PlayerType>())
                .Returns(new[]
                {
                    Status.Active,
                    Status.Maintenance
                });

            fakeValidGameStatus.GetStatus(Arg.Any<PlayerType>())
                .Returns(new[]
                {
                    GameStatus.Active,
                    GameStatus.Maintenance,
                    GameStatus.Testing
                });

            fakeGameProviderRepository.QueryAllSubGameProvidersAsync(Arg.Is(brandId))
                .Returns(subGameProviders.ToAsyncEnumerable());

            fakeGameProviderRepository.QueryAllGamesAsync(Arg.Is(brandId))
                .Returns(games.ToAsyncEnumerable());

            var sut = new GetAllGamesByGameTypeQueryService(
                fakeValidStatus,
                fakeValidGameStatus,
                fakeGameProviderRepository);

            var gameTypes = new[]
            {
                GameType.Slot
            };

            var query = new GetAllGamesByGameTypeQuery(
                brandId,
                PlayerType.RealPlayer,
                PlatformType.Desktop,
                gameTypes);

            // act
            var result = await sut.QueryAsync(query).ToArrayAsync();

            // assert
            var expected = games.Where(g => gameTypes.Contains(g.Type)).ToArray();

            Assert.AreEqual(expected.Length, result.Length);
            Assert.AreNotEqual(games.Length, result.Length);
        }

        [Test]
        public async Task Get_Game_Url_With_Extend_Data()
        {
            // arrange
            var brandId = Guid.NewGuid();
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

            var gameProvider = new CachedGameProvider
            {
                Id = gameProviderId,
                Code = "EVO",
                Status = Status.Active
            };

            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();
            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakePluggalbeGameProviderApiClient = Substitute.For<IPluggalbeGameProviderApiClient>();
            var fakePluggableOperations = Substitute.For<IPluggableOperations>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

            fakeGameProviderRepository.GetSubGameProviderDataInfoAsync(Arg.Is(subGameProviderCode))
                .Returns(ValueTask.FromResult(new SubGameProviderWithLocationDataInfo(
                    gameProviderId,
                    "EVO",
                    "RT",
                    "RT",
                    "RT",
                    "{}")));

            fakeGameProviderOperations
                .GetCachedGameProviderAsync(gameProviderId)
                .Returns(Task.FromResult(gameProvider));

            fakeGameProviderOperations
                .GetGamePlatformTypeAsync(gameProviderId, gameId)
                .Returns(Task.FromResult((GamePlatformType?)GamePlatformType.Desktop));

            fakePlayerOperations
                .GetCachedPlayerAsync(playerCode)
                .Returns(Task.FromResult(new CachedPlayer
                {
                    Status = PlayerStatus.Enabled
                }));

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
                brandId,
                playerToken,
                PlayerType.RealPlayer,
                subGameProviderCode,
                gameId,
                languageCode,
                extendedData);

            var sut = new GetGameUrlQueryService(
                fakeStatusValidator,
                fakeGameProviderOperations,
                fakeValidStatus,
                fakeValidGameStatus,
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
                Guid.NewGuid(),
                gameProviderCode,
                playerToken,
                PlayerType.RealPlayer,
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

        [Test]
        public async Task Player_Can_Add_Favorite_Game()
        {
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

            var sut = new SetFavoriteGameCommandService(fakeGameProviderRepository);

            var command = new SetFavoriteGameCommand(
                Guid.NewGuid(),
                "ThisIsPlayerCode",
                1000);

            fakeGameProviderRepository.QueryAllGamesAsync(Arg.Is(command.BrandId))
                .Returns(new[]
                {
                    new BrandGameInfo(
                        command.GameId,
                        "code1",
                        "name1",
                        "test",
                        "subgp1",
                        Guid.NewGuid(),
                        "gp1",
                        GameStatus.Active,
                        GameStatus.Active,
                        GameType.Arcade,
                        GamePlatformType.Desktop,
                        true,
                        string.Empty,
                        string.Empty,
                        GameTag.New,
                        0)
                }.ToAsyncEnumerable());

            await sut.ExecuteAsync(command);

            _ = fakeGameProviderRepository.Received(1)
                .AddFavoriteGameAsync(Arg.Is(command.BrandId), Arg.Any<FavoriteGameInfo>());
        }

        [Test]
        public async Task Player_Can_Remove_Favorite_Game()
        {
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

            var sut = new RemoveFavoriteGameCommandService(fakeGameProviderRepository);

            var command = new RemoveFavoriteGameCommand("ThisIsPlayerCode", 1000);

            await sut.ExecuteAsync(command);

            _ = fakeGameProviderRepository.Received(1)
                .RemoveFavoriteGameAsync(Arg.Is(command.PlayerCode), Arg.Is(command.GameId));
        }

        [Test]
        public void Player_Can_List_Favorite_Games()
        {
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

            var sut = new ListFavoriteGameIdsQueryService(fakeGameProviderRepository);

            var query = new ListFavoriteGameIdsQuery(
                Guid.NewGuid(),
                "ThisIsPlayerCode",
                GameType.Arcade);

            sut.QueryAsync(query);

            fakeGameProviderRepository.Received(1)
                .QueryPlayerFavoriteGamesAsync(
                    Arg.Is(query.BrandId),
                    Arg.Is(query.PlayerCode));
        }

        [Test]
        public async Task Can_Count_Player_Favorite_Games()
        {
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

            var sut = new CountPlayerFavoriteGameQueryService(fakeGameProviderRepository);

            var query = new CountPlayerFavoriteGameQuery(
                Guid.NewGuid(),
                "ThisIsPlayerCode");

            await sut.GetAsync(query);

            _ = fakeGameProviderRepository.Received(1)
                .QueryPlayerFavoriteGamesAsync(
                    Arg.Is(query.BrandId),
                    Arg.Is(query.PlayerCode));
        }

        [Test]
        public async Task Should_Fund_In_All_Game_Balance_Before_Get_Game_Url()
        {
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakeBrandLanguageCodeMapper = Substitute.For<IBrandLanguageCodeMapper>();
            var fakeGetGpCodeService = Substitute.For<IGetService<GetGpCodeQuery, BrandSubGameProviderInfo>>();
            var fakeGetGameUrlQueryService = Substitute.For<IGetService<GetGameUrlQuery, GameUrlInfo>>();
            var fakeFundInAllService = Substitute.For<ICommandService<FundInAllGameBalanceCommand>>();
            var fakeFundService = Substitute.For<ICommandService<FundOutBeforeEnterGameCommand>>();

            var playerTokenData = new PlayerTokenData
            {
                PlayerCode = "playerCode",
                PlatformType = PlatformType.Desktop,
                LoginRecordId = Guid.NewGuid(),
                ExpiredOn = DateTimeOffset.Now
            };

            var httpContext = new DefaultHttpContext();
            httpContext.Items.Add("playerToken", playerTokenData);
            httpContext.Items.Add("brandId", Guid.NewGuid());

            fakePlayerOperations.GetCachedPlayerAsync(Arg.Any<string>())
                .Returns(Task.FromResult(new CachedPlayer
                {
                    PlayerCode = playerTokenData.PlayerCode,
                    Status = PlayerStatus.Enabled
                }));

            var sut = new GameController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            var request = new GetGameUrlRequest
            {
                SubGameProviderCode = "sub1",
                GameId = 10,
                LanguageCode = "zh-cn"
            };

            fakeGetGpCodeService.GetAsync(Arg.Any<GetGpCodeQuery>())
                .Returns(ValueTask.FromResult(new BrandSubGameProviderInfo(
                    "sub1",
                    "sub1",
                    "sub1",
                    Guid.NewGuid(),
                    "gp1",
                    true,
                    Status.Active,
                    Status.Active)));

            _ = fakeGetGameUrlQueryService.GetAsync(Arg.Any<GetGameUrlQuery>())
                .Returns(ValueTask.FromResult(new GameUrlInfo(0, string.Empty, "http://www.google.com")));

            await sut.GetGameUrlForPostAsync(
                fakePlayerOperations,
                fakeStatusValidator,
                fakeBrandLanguageCodeMapper,
                fakeGetGpCodeService,
                fakeGetGameUrlQueryService,
                fakeFundInAllService,
                fakeFundService,
                request);

            _ = fakeFundInAllService.Received(1)
                .ExecuteAsync(Arg.Any<FundInAllGameBalanceCommand>());
        }

        [Test]
        public async Task Should_Fund_In_All_Game_Balance_Before_Get_Lobby_Game_Url()
        {
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeStatusValidator = Substitute.For<IStatusValidator>();
            var fakeBrandLanguageCodeMapper = Substitute.For<IBrandLanguageCodeMapper>();
            var fakeGetGpCodeService = Substitute.For<IGetService<GetGpCodeQuery, BrandSubGameProviderInfo>>();
            var fakeGetLobbyGameUrlQueryService = Substitute.For<IGetService<GetLobbyGameUrlQuery, LobbyGameUrlInfo>>();
            var fakeFundInAllService = Substitute.For<ICommandService<FundInAllGameBalanceCommand>>();
            var fakeFundService = Substitute.For<ICommandService<FundOutBeforeEnterGameCommand>>();

            var playerTokenData = new PlayerTokenData
            {
                PlayerCode = "playerCode",
                PlatformType = PlatformType.Desktop,
                LoginRecordId = Guid.NewGuid(),
                ExpiredOn = DateTimeOffset.Now
            };

            var httpContext = new DefaultHttpContext();
            httpContext.Items.Add("playerToken", playerTokenData);
            httpContext.Items.Add("brandId", Guid.NewGuid());

            fakePlayerOperations.GetCachedPlayerAsync(Arg.Any<string>())
                .Returns(Task.FromResult(new CachedPlayer
                {
                    PlayerCode = playerTokenData.PlayerCode,
                    Status = PlayerStatus.Enabled
                }));

            var sut = new GameController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            var request = new GetLobbyGameUrlRequest
            {
                SubGameProviderCode = "sub1",
                LanguageCode = "zh-cn"
            };

            fakeGetGpCodeService.GetAsync(Arg.Any<GetGpCodeQuery>())
                .Returns(ValueTask.FromResult(new BrandSubGameProviderInfo(
                    "sub1",
                    "sub1",
                    "sub1",
                    Guid.NewGuid(),
                    "gp1",
                    true,
                    Status.Active,
                    Status.Active)));

            _ = fakeGetLobbyGameUrlQueryService.GetAsync(Arg.Any<GetLobbyGameUrlQuery>())
                .Returns(ValueTask.FromResult(new LobbyGameUrlInfo(0, string.Empty, "http://www.google.com")));

            await sut.GetLobbyGameUrlForPostAsync(
                fakePlayerOperations,
                fakeStatusValidator,
                fakeBrandLanguageCodeMapper,
                fakeGetGpCodeService,
                fakeGetLobbyGameUrlQueryService,
                fakeFundInAllService,
                fakeFundService,
                request);

            _ = fakeFundInAllService.Received(1)
                .ExecuteAsync(Arg.Any<FundInAllGameBalanceCommand>());
        }
    }
}