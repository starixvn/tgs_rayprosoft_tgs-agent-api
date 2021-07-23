using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Migrations;
using TGS.Core.Infrastructures.Extensions;
using TGS.Services.BackOffice.Application.Queries.GameProviderHandlers;
using TGS.Services.BackOffice.Application.Queries.GameProviders;
using CoreModels = TGS.Core.DataServices.Tgs.Models;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class GameProvidersLocalizationTests
    {
        [Test]
        public async Task GameProviders_Language_Should_Get_Distinct_Culture_Code_Count()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            var repository = new TgsRepository(dbContextOptionsBuilder.Options);

            await repository.GameProviderLanguages.AddRangeAsync(
                new CoreModels.GameProviderLanguage()
                {
                    GameProviderId = Guid.NewGuid(),
                    CultureCode = "en-US"
                },
                new CoreModels.GameProviderLanguage()
                {
                    GameProviderId = Guid.NewGuid(),
                    CultureCode = "en-US"
                },
                new CoreModels.GameProviderLanguage()
                {
                    GameProviderId = Guid.NewGuid(),
                    CultureCode = "zh-CN"
                });
            await repository.SaveChangesAsync();

            var sut = new GetAllSupportLanguagesQueryService(repository);

            var actual = await sut.QueryAsync(new GetAllSupportLanguagesQuery()).CountAsync();

            Assert.AreEqual(2, actual);
        }

        [Test]
        public async Task Can_Query_SubGameProvider_Data()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            var repository = new TgsRepository(dbContextOptionsBuilder.Options);

            await repository.SubGameProviders.AddRangeAsync(
                new CoreModels.SubGameProvider
                {
                    Code = "Test"
                });
            await repository.SaveChangesAsync();

            var sut = new GetGameProvidersLocalizationQueryService(repository);

            var actual = await sut.QueryAsync(new GetGameProvidersLocalizationQuery()).CountAsync();

            Assert.AreEqual(1, actual);
        }

        [Test]
        public async Task Update_Game_Prodvier_Location_With_Exist_Id_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);
            var gameProviderId = new Guid("5daca63e-4265-4791-adda-cf6e935d39e0");

            repository.GameProviders.Add(
                new CoreModels.GameProvider()
                {
                    Id = gameProviderId
                });
            await repository.SaveChangesAsync();

            var sut = new UpdateGameProvidersLocalizationQueryService(repository);

            var localization = new Dictionary<string, JsonDocument>()
            {
                { "en-US", JsonDocument.Parse("{ \"name\": \"Happy\" }") },
                { "zh-CN", JsonDocument.Parse("{ \"name\": \"開心\" }") },
                { "zh-TW", JsonDocument.Parse("{ \"name\": \"开心\" }") }
            };

            var query = new UpdateGameProvidersLocalizationQuery()
            {
                GameProviders = new List<GameProvidersLocalizationModel>() {
                    new GameProvidersLocalizationModel() {
                        Id = gameProviderId,
                        Localization = localization
                    }}
            };

            await sut.GetAsync(query, default);

            var actual = repository.GameProviders.FirstOrDefault(x => x.Id == gameProviderId);

            Assert.AreEqual(localization.ToJson(), actual.Localization);
        }

        [Test]
        public async Task Update_Game_Provider_Localization_With_NonExist_Id_Should_Failed()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var existGameProviderId = new Guid("5daca63e-4265-4791-adda-cf6e935d39e0");
            repository.GameProviders.Add(
                new CoreModels.GameProvider()
                {
                    Id = existGameProviderId
                });
            await repository.SaveChangesAsync();

            var sut = new UpdateGameProvidersLocalizationQueryService(repository);

            var localization = new Dictionary<string, JsonDocument>()
            {
                { "en-US", JsonDocument.Parse("{ \"name\": \"Happy\" }") },
                { "zh-CN", JsonDocument.Parse("{ \"name\": \"開心\" }") },
                { "zh-TW", JsonDocument.Parse("{ \"name\": \"开心\" }") }
            };

            var query = new UpdateGameProvidersLocalizationQuery()
            {
                GameProviders = new List<GameProvidersLocalizationModel>() {
                    new GameProvidersLocalizationModel() {
                        Id = new Guid("09d8d244-206b-4030-8110-b2ea90dc9d2f"),
                        Localization = localization
                    }}
            };

            await sut.GetAsync(query, default);

            var actual = repository.GameProviders.FirstOrDefault(x => x.Id == existGameProviderId);

            Assert.AreNotEqual(localization.ToJson(), actual.Localization);
        }
    }
}