using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.GameBalance;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using CoreModesl = TGS.Core.DataServices.Tgs.Models;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class GameBalanceTests
    {
        [Test]
        public async Task Player_Can_Fund_In()
        {
            // arrange
            var fakePluggalbeGameProviderApiClient = Substitute.For<IPluggalbeGameProviderApiClient>();
            var fakePluggableOperations = Substitute.For<IPluggableOperations>();
            var fakeLogger = new NullLogger<FundInGameBalanceCommandService>();

            var gameProviderCode = "KY";
            var playerCode = "playerCode";
            var fakeUri = new Uri("https://www.fake.com");
            int amount = 10000;
            var request = new GameProviderWalletFundRequest()
            {
                GameProviderCode = gameProviderCode,
                PlayerCode = playerCode,
                Amount = amount
            };
            fakePluggableOperations
                .GetEndpointUrlByGameProviderCodeAsync(gameProviderCode)
                .Returns(fakeUri);

            fakePluggalbeGameProviderApiClient
                .FundInAsync(fakeUri, Arg.Any<GameProviderWalletFundRequest>())
                .Returns(new GameProviderWalletFundResponse()
                {
                    Error = 0,
                    ErrorMessage = string.Empty
                });

            var sut = new FundInGameBalanceCommandService(fakePluggalbeGameProviderApiClient, fakePluggableOperations, fakeLogger);

            // act
            var command = new FundInGameBalanceCommand(gameProviderCode, playerCode, amount);
            await sut.ExecuteAsync(command, default);

            // assert
            _ = fakePluggalbeGameProviderApiClient
                .Received(1)
                .FundInAsync(Arg.Is(fakeUri), Arg.Is(request));
        }

        [Test]
        public async Task Player_Can_Fund_Out()
        {
            // arrange
            var gameProviderCode = "KY";
            var playerCode = "playerCode";
            int amount = 10000;
            var fakeUri = new Uri("https://www.fake.com");

            var fakePluggalbeGameProviderApiClient = Substitute.For<IPluggalbeGameProviderApiClient>();
            var fakePluggableOperations = Substitute.For<IPluggableOperations>();
            var fakeLogger = new NullLogger<FundOutGameBalanceCommandService>();

            fakePluggableOperations
                .GetEndpointUrlByGameProviderCodeAsync(gameProviderCode)
                .Returns(fakeUri);

            fakePluggalbeGameProviderApiClient
                .FundOutAsync(fakeUri, Arg.Any<GameProviderWalletFundRequest>())
                .Returns(new GameProviderWalletFundResponse()
                {
                    Error = 0,
                    ErrorMessage = string.Empty
                });

            var request = new GameProviderWalletFundRequest()
            {
                GameProviderCode = gameProviderCode,
                PlayerCode = playerCode,
                Amount = amount
            };
            var sut = new FundOutGameBalanceCommandService(fakePluggalbeGameProviderApiClient, fakePluggableOperations, fakeLogger);

            // act
            var command = new FundOutGameBalanceCommand(gameProviderCode, playerCode, amount);
            await sut.ExecuteAsync(command, default);

            // assert
            _ = fakePluggalbeGameProviderApiClient
                .Received(1)
                .FundOutAsync(Arg.Is(fakeUri), Arg.Is(request));
        }

        [Test]
        public async Task Player_Can_Get_Transfer_Wallet_Balance()
        {
            // arrange
            var playerCode = "playerCode";
            var gameProviderCode = "KY";
            var fakeUri = new Uri("https://www.fake.com");

            var fakePluggalbeGameProviderApiClient = Substitute.For<IPluggalbeGameProviderApiClient>();
            var fakePluggableOperations = Substitute.For<IPluggableOperations>();

            var request = new GameProviderWalletGetBalanceRequest()
            {
                GameProviderCode = gameProviderCode,
                PlayerCode = playerCode
            };

            fakePluggableOperations
                .GetEndpointUrlByGameProviderCodeAsync(gameProviderCode)
                .Returns(fakeUri);
            fakePluggalbeGameProviderApiClient
                .GetBalanceAsync(fakeUri, request)
                .Returns(new GameProviderWalletGetBalanceResponse() { Balance = 1000 });

            var sut = new GetGameBalanceQueryService(
                fakePluggalbeGameProviderApiClient,
                fakePluggableOperations);

            // act
            var query = new GetGameBalanceQuery(playerCode, gameProviderCode);
            await sut.GetAsync(query, default);

            // assert
            _ = fakePluggalbeGameProviderApiClient
                .Received(1)
                .GetBalanceAsync(Arg.Is(fakeUri), Arg.Is(request));
        }

        [Test]
        public void Player_Can_Not_Use_Transfer_Wallet_Balance_Query_FundIn_FundOut_When_Brand_GameProvider_Status_Is_Inactive()
        {
            // arrange
            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var playerCode = "playerCode";
            var brandId = Guid.NewGuid();
            var gameProviderCode = "ae5533bd-35d5-5478-19d6-233ed44f7c9d";
            var gameProviderId = Guid.NewGuid();
            var gameProviderStatus = Status.Active;

            fakeGameProviderOperations
                .GetGameProviderIdByCodeAsync(Arg.Any<string>())
                .Returns(gameProviderId);
            fakeGameProviderOperations
                .GetGameProviderStatusAsync(Arg.Any<Guid>())
                .Returns(gameProviderStatus);

            var cachedPlayer = new CachedPlayer() { Type = PlayerType.RealPlayer };
            fakePlayerOperations
                .GetCachedPlayerAsync(playerCode)
                .Returns(cachedPlayer);

            var playerTokenData = new PlayerTokenData
            {
                PlayerCode = playerCode
            };

            fakeValidStatus.GetStatus(cachedPlayer.Type).Returns(new Status[] { Status.Active });
            var brandGameProviderStatus = Status.Inactive;

            fakeBrandOperations.GetBrandGameProviderStatusAsync(brandId, gameProviderId).Returns(brandGameProviderStatus);

            var sut = new GameProviderStatusValidator(fakeGameProviderOperations, fakePlayerOperations, fakeValidStatus, fakeBrandOperations);

            // assert
            Assert.ThrowsAsync<InvalidStatusException>(async () => await sut.CheckGameProviderStatusAsync(playerTokenData, gameProviderCode, brandId));
        }

        [Test]
        public void Player_Can_Not_Use_Transfer_Wallet_Balance_Query_FundIn_FundOut_When_Brand_GameProvider_Status_Is_Maintenance()
        {
            // arrange
            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var playerCode = "playerCode";
            var brandId = Guid.NewGuid();
            var gameProviderCode = "ae5533bd-35d5-5478-19d6-233ed44f7c9d";
            var gameProviderId = Guid.NewGuid();
            var gameProviderStatus = Status.Active;

            fakeGameProviderOperations
                .GetGameProviderIdByCodeAsync(Arg.Any<string>())
                .Returns(gameProviderId);
            fakeGameProviderOperations
                .GetGameProviderStatusAsync(Arg.Any<Guid>())
                .Returns(gameProviderStatus);

            var cachedPlayer = new CachedPlayer() { Type = PlayerType.RealPlayer };
            fakePlayerOperations
                .GetCachedPlayerAsync(playerCode)
                .Returns(cachedPlayer);

            var playerTokenData = new PlayerTokenData
            {
                PlayerCode = playerCode
            };

            fakeValidStatus.GetStatus(cachedPlayer.Type).Returns(new Status[] { Status.Active });
            var brandGameProviderStatus = Status.Maintenance;

            fakeBrandOperations.GetBrandGameProviderStatusAsync(brandId, gameProviderId).Returns(brandGameProviderStatus);

            var sut = new GameProviderStatusValidator(fakeGameProviderOperations, fakePlayerOperations, fakeValidStatus, fakeBrandOperations);

            // assert
            Assert.ThrowsAsync<GameProviderUnderMaintenanceException>(async () => await sut.CheckGameProviderStatusAsync(playerTokenData, gameProviderCode, brandId));
        }

        [Test]
        public void Player_Can_Not_Use_Transfer_Wallet_Balance_Query_FundIn_FundOut_When_GameProvider_Status_Is_Inactive()
        {
            // arrange
            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var playerCode = "playerCode";
            var brandId = Guid.NewGuid();
            var gameProviderCode = "ae5533bd-35d5-5478-19d6-233ed44f7c9d";
            var gameProviderId = Guid.NewGuid();
            var gameProviderStatus = Status.Inactive;

            fakeGameProviderOperations
                .GetGameProviderIdByCodeAsync(Arg.Any<string>())
                .Returns(gameProviderId);
            fakeGameProviderOperations
                .GetGameProviderStatusAsync(Arg.Any<Guid>())
                .Returns(gameProviderStatus);

            var cachedPlayer = new CachedPlayer() { Type = PlayerType.RealPlayer };
            fakePlayerOperations
                .GetCachedPlayerAsync(playerCode)
                .Returns(cachedPlayer);

            var playerTokenData = new PlayerTokenData
            {
                PlayerCode = playerCode
            };

            var sut = new GameProviderStatusValidator(fakeGameProviderOperations, fakePlayerOperations, fakeValidStatus, fakeBrandOperations);

            // assert
            Assert.ThrowsAsync<InvalidStatusException>(async () => await sut.CheckGameProviderStatusAsync(playerTokenData, gameProviderCode, brandId));
        }

        [Test]
        public void Player_Can_Not_Use_Transfer_Wallet_Balance_Query_FundIn_FundOut_When_GameProvider_Status_Is_Maintenance()
        {
            // arrange
            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var playerCode = "playerCode";
            var brandId = Guid.NewGuid();
            var gameProviderCode = "ae5533bd-35d5-5478-19d6-233ed44f7c9d";
            var gameProviderId = Guid.NewGuid();
            var gameProviderStatus = Status.Maintenance;

            fakeGameProviderOperations
                .GetGameProviderIdByCodeAsync(Arg.Any<string>())
                .Returns(gameProviderId);
            fakeGameProviderOperations
                .GetGameProviderStatusAsync(Arg.Any<Guid>())
                .Returns(gameProviderStatus);

            var cachedPlayer = new CachedPlayer() { Type = PlayerType.RealPlayer };
            fakePlayerOperations
                .GetCachedPlayerAsync(playerCode)
                .Returns(cachedPlayer);

            var playerTokenData = new PlayerTokenData
            {
                PlayerCode = playerCode
            };

            var sut = new GameProviderStatusValidator(fakeGameProviderOperations, fakePlayerOperations, fakeValidStatus, fakeBrandOperations);

            // assert
            Assert.ThrowsAsync<GameProviderUnderMaintenanceException>(async () => await sut.CheckGameProviderStatusAsync(playerTokenData, gameProviderCode, brandId));
        }

        [Test]
        public async Task Player_Can_Get_Game_Balance_Infos()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);
            var brandId = Guid.NewGuid();

            repository.Brands.Add(new CoreModesl.Brand()
            {
                Id = brandId,
                Name = "brandName"
            });
            await repository.SaveChangesAsync();

            var sut = new GetGameBalanceInfosQueryService(repository);

            var query = new GetGameBalanceInfosQuery(
                BrandId: brandId,
                PlayerCode: "playerCode",
                RecordOnRange: null);

            var info = await sut.QueryAsync(query).ToArrayAsync();

            Assert.IsTrue(info.Length >= 0);
        }
    }
}