using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using TGS.Apis.OperatorApi.Handlers.Game;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;

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
    }
}