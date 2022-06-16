using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations;
using TGS.Services.GameOperations.Models.Data;
using TGS.Services.Operations;

namespace TGS.UnitTests.Operations;

[TestFixture]
public class GameAccessorTests
{
    [Test]
    public async Task Can_Get_Brand_Available_Games()
    {
        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        var gamePlatformTypes = new[] { GamePlatformType.Desktop, GamePlatformType.DesktopAndMobile };

        var gameInfo = new GameInfo(11, "1_1", GameStatus.Active);

        var expected = new GameInfo[]
        {
            gameInfo
        };

        var brandGames = new CachedBrandGame[]
        {
            new CachedBrandGame
            {
                GameId = gameInfo.Id,
                BrandGameStatus = gameInfo.Status,
                ExternalGameId = gameInfo.ExternalId,
                PlatformType = GamePlatformType.DesktopAndMobile,
                GameProviderId = gameProviderId
            },
            new CachedBrandGame
            {
                GameId = 21,
                BrandGameStatus = GameStatus.Active,
                ExternalGameId = "2_1",
                PlatformType = GamePlatformType.DesktopAndMobile,
                GameProviderId = gameProviderId
            }
        };

        var games = new CachedGame[]
        {
            new CachedGame
            {
                Id = gameInfo.Id,
                Status = gameInfo.Status,
                ExternalId = gameInfo.ExternalId,
                PlatformType = GamePlatformType.DesktopAndMobile
            },
            new CachedGame
            {
                Id = 12,
                Status = GameStatus.Active,
                ExternalId = "1_2",
                PlatformType = GamePlatformType.DesktopAndMobile
            },
            new CachedGame
            {
                Id = 13,
                Status = GameStatus.Inactive,
                ExternalId = "1_3",
                SubGame = true,
                AvailableToBrand = true,
                PlatformType = GamePlatformType.DesktopAndMobile
            }
        };

        var brandOperationsMock = new Mock<IBrandOperations>();
        var gameProviderOperationsMock = new Mock<IGameProviderOperations>();

        brandOperationsMock
            .Setup(x
                => x.GetBrandGamesAsync(brandId, gameProviderId, gamePlatformTypes))
            .Returns(Task.FromResult(brandGames));

        gameProviderOperationsMock
            .Setup(x
                => x.GetCachedGamesAsync(gameProviderId, gamePlatformTypes))
            .Returns(Task.FromResult(games));

        var sut = new GameAccessor(brandOperationsMock.Object, gameProviderOperationsMock.Object);

        var actual = (await sut
            .GetBrandAvailableGamesAsync(brandId, gameProviderId, new[] { gameInfo.ExternalId }, gamePlatformTypes)
            .ConfigureAwait(false)).ToArray();

        CollectionAssert.AreEqual(expected, actual);
    }
}
