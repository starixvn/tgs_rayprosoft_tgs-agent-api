using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations;
using TGS.Services.Operations;

namespace TGS.UnitTests.Operations
{
    [TestFixture]
    public class GameAccessorTests
    {
        [Test]
        public async Task Can_Get_Brand_Available_Games()
        {
            var brandId = Guid.NewGuid();
            var gameProviderId = Guid.NewGuid();
            var gameExternalIds = new[] { "1_1" };
            var gamePlatformTypes = new[] { GamePlatformType.Desktop, GamePlatformType.DesktopAndMobile };

            var brandGames = new CachedBrandGame[]
            {
                new CachedBrandGame
                {
                    GameId = 11,
                    BrandGameStatus = GameStatus.Active,
                    ExternalGameId = "1_1",
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
                    Id = 11,
                    Status = GameStatus.Active,
                    ExternalId = "1_1",
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
                    Status = GameStatus.Active,
                    ExternalId = "1_3",
                    PlatformType = GamePlatformType.DesktopAndMobile
                }
            };

            var subGames = new CachedSubGame[]
            {
                new CachedSubGame
                {
                    Id = 71,
                    ExternalId = "7_1",
                    PlatformType = GamePlatformType.DesktopAndMobile
                },
                new CachedSubGame
                {
                    Id = 91,
                    ExternalId = "9_1",
                    PlatformType = GamePlatformType.Desktop
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

            gameProviderOperationsMock
                .Setup(x
                    => x.GetSubGamesAsync(gameProviderId, gamePlatformTypes))
                .Returns(Task.FromResult(subGames));

            var sut = new GameAccessor(brandOperationsMock.Object, gameProviderOperationsMock.Object);

            var availableGames = await sut
                .GetBrandAvailableGamesAsync(brandId, gameProviderId, gameExternalIds, gamePlatformTypes)
                .ConfigureAwait(false);

            var assert = gameExternalIds.All(id => availableGames.Any(ag => ag.ExternalId == id));

            Assert.AreEqual(true, assert);
        }
    }
}
