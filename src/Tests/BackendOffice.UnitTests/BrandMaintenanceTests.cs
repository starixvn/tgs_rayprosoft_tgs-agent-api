using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.BackOffice.Application.Models.Brand;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Tests.BackendOffice.UnitTests;

[TestFixture]
public class BrandMaintenanceTests
{
    [Test]
    public async Task User_Can_Update_All_Brand_To_Maintenance_Status()
    {
        var moqBrandSerivce = new Mock<IBrandService>();

        var sut = new UpdateAllBrandMaintenanceStatusCommandService(moqBrandSerivce.Object);

        var command = new UpdateAllBrandMaintenanceStatusCommand("editorUserName");

        var list = CreateBrandFixture()
           .CreateMany<Brand>(3)
           .AsQueryable()
           .BuildMock();

        moqBrandSerivce
            .Setup(b => b.GetBrandQueries(It.IsAny<Func<IQueryable<Brand>, IQueryable<Brand>>>(), false))
            .Returns(list);

        await sut.ExecuteAsync(command);

        moqBrandSerivce.Verify(
            fake => fake.ChangeStatusAsync(
                It.IsAny<Brand>(),
                It.IsAny<UpdateBrandStatusRequest>()),
            Times.Exactly(3));
    }

    private Fixture CreateBrandFixture()
    {
        var fixture = new Fixture();

        fixture.Customize<Brand>(u => u
            .With(b => b.Licensee, new Licensee())
            .With(b => b.Announcements, Array.Empty<Announcement>())
            .With(b => b.BetLimits, Array.Empty<BrandBetLimit>())
            .With(b => b.BrandGroups, Array.Empty<BrandGroup>())
            .With(b => b.BrandAgentDividendConditions, Array.Empty<BrandAgentDividendCondition>())
            .With(b => b.Currencies, Array.Empty<BrandCurrency>())
            .With(b => b.Currencies, Array.Empty<BrandCurrency>())
            .With(b => b.Departments, Array.Empty<Department>())
            .With(b => b.Domains, Array.Empty<BrandDomain>())
            .With(b => b.GameProviders, Array.Empty<BrandGameProvider>())
            .With(b => b.Games, Array.Empty<BrandGame>())
            .With(b => b.Languages, Array.Empty<BrandLanguage>())
            .With(b => b.PlayerLoginRecords, Array.Empty<PlayerLoginRecord>())
            .With(b => b.Players, Array.Empty<Player>())
            .With(b => b.StationLetters, Array.Empty<StationLetter>())
            .With(b => b.VipLevels, Array.Empty<VipLevel>())
            );

        return fixture;
    }
}
