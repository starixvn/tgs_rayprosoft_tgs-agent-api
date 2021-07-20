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
using TGS.Services.Operator.GameProviders;
using TGS.Services.Operator.Games;
using TGS.Services.Operator.Services.GameProvider;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class GameProviderTests
    {
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
            var fakeGameRepository = Substitute.For<IGameRepository>();

            var sut = new GetGameProvidersQueryService(
                fakeBrandCurrencyCodeMapper,
                fakePlayerOperations,
                fakeBrandLanguageCodeMapper,
                fakeBrandOperations,
                fakeValidStatus,
                fakeValidGameStatus,
                fakeGameProviderRepository,
                fakeGameRepository);

            fakeBrandOperations.GetBrandGameProvidersAsync(brandId)
                .Returns(Task.FromResult(brandGameProviders));

            // act
            await sut.QueryAsync(query).ToArrayAsync();

            _ = fakeGameProviderRepository.Received(1)
                .QueryGameProvidersAsync(Arg.Any<Guid[]>(), Arg.Any<Status[]>());
            _ = fakeGameProviderRepository.Received(1).QuerySubGameProvidersAsync(Arg.Any<Guid[]>());
        }
    }
}