using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operator;
using TGS.Services.Operator.GameProviders;
using TGS.Services.Operator.Services.Game;
using TGS.Services.Operator.Services.GameProvider;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class GameProviderTests
    {
        [Test]
        public async Task Can_Find_GpCode_By_SubGpCode()
        {
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameProviderOperations = Substitute.For<IGameProviderOperations>();

            var sut = new GetGpCodeQueryService(
                fakeGameProviderRepository,
                fakeGameProviderOperations);

            var query = new GetGpCodeQuery("SubGpCode");

            fakeGameProviderRepository.GetSubGameProviderDataInfoAsync(Arg.Is(query.SubGpCode))
                .Returns(new SubGameProviderWithLocationDataInfo(
                    Guid.NewGuid(),
                    "GpCode",
                    "SubGpCode",
                    "test",
                    "test",
                    "{}"));

            var expected = "GpCode";

            var actual = await sut.GetAsync(query);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task Get_GameProvider_List()
        {
            var brandId = Guid.NewGuid();

            var brandGameProviders = new CachedBrandGameProvider[]
            {
                new()
                {
                    GameProviderId = Guid.NewGuid(),
                    BrandGameProviderStatus = Status.Active
                },
                new()
                {
                    GameProviderId = Guid.NewGuid(),
                    BrandGameProviderStatus = Status.Active
                }
            };

            var query = new GetGameProvidersQuery(
                brandId,
                "CNY",
                new PlayerTokenData(),
                PlatformType.Desktop,
                "zh-CN");

            var fakeBrandCurrencyCodeMapper = Substitute.For<IBrandCurrencyCodeMapper>();
            var fakeBrandOperations = Substitute.For<IBrandOperations>();
            var fakePlayerOperations = Substitute.For<IPlayerOperations>();
            var fakeBrandLanguageCodeMapper = Substitute.For<IBrandLanguageCodeMapper>();
            var fakeValidStatus = Substitute.For<IValidStatus<Status>>();
            var fakeValidGameStatus = Substitute.For<IValidStatus<GameStatus>>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();

            var sut = new GetGameProvidersQueryService(
                fakeBrandCurrencyCodeMapper,
                fakePlayerOperations,
                fakeBrandLanguageCodeMapper,
                fakeBrandOperations,
                fakeValidStatus,
                fakeValidGameStatus,
                fakeGameProviderRepository);

            fakeBrandOperations.GetBrandGameProvidersAsync(brandId)
                .Returns(Task.FromResult(brandGameProviders));

            // act
            await sut.QueryAsync(query).ToArrayAsync();

            _ = fakeGameProviderRepository.Received(1)
                .QueryGameProvidersAsync();
            _ = fakeGameProviderRepository.Received(1)
                .QuerySubGameProvidersAsync();
        }
    }
}
