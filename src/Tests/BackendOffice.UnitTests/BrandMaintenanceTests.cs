using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Paging;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Tests.BackendOffice.UnitTests;

[TestFixture]
public class BrandMaintenanceTests
{
    [Test]
    public async Task User_Can_Count_BrandMaintenance_Query()
    {
        var moqBackOfficeRepository = new Mock<IBackOfficeRepository>();

        var sut = new GetAllBrandMaintenanceQueryService(moqBackOfficeRepository.Object);

        var paging = new Paging<BrandMaintenanceInfo>
        {
            RowCount = 10,
            Page = 0
        };

        paging.SortByPropertyName(nameof(BrandMaintenanceInfo.BrandCode), false);

        var query = new GetAllBrandMaintenanceQuery(
            "",
            paging);

        moqBackOfficeRepository
            .Setup(fake => fake.QueryBrandMaintenanceByBrandCodeAsync(It.Is<string>(x => x == query.BrandCode)))
            .Returns(new[]
            {
                new BrandMaintenanceInfo(
                    "GameProvider Integration",
                    "GPI",
                    "Turnkey Game System",
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow),
                new BrandMaintenanceInfo(
                    "GPI_PHP",
                    "GPI_PHP",
                    "Turnkey Game System",
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow)
            }.ToAsyncEnumerable());

        var actual = await sut.GetAsync(query);

        Assert.AreEqual(2, actual);
    }

    [Test]
    public async Task User_Can_Get_a_Brand_Maintenance()
    {
        var moqBackOfficeRepository = new Mock<IBackOfficeRepository>();

        var sut = new GetBrandMaintenanceQueryService(moqBackOfficeRepository.Object);

        var id = Guid.NewGuid();
        var query = new GetBrandMaintenanceQuery(id);

        moqBackOfficeRepository
            .Setup(fake => fake.GetBrandMaintenanceByBrandCodeAsync(It.Is<Guid>(x => x == query.Id)))
            .Returns(ValueTask.FromResult(new BrandMaintenanceModel("This is Content")));

        await sut.GetAsync(query);

        moqBackOfficeRepository.Verify(
            fake => fake.GetBrandMaintenanceByBrandCodeAsync(It.Is<Guid>(x => x == query.Id)),
            Times.Exactly(1));
    }

    [Test]
    public async Task User_Can_Query_Brand_Maintenance()
    {
        var moqBackOfficeRepository = new Mock<IBackOfficeRepository>();

        var sut = new GetAllBrandMaintenanceQueryService(moqBackOfficeRepository.Object);

        var paging = new Paging<BrandMaintenanceInfo>
        {
            RowCount = 10,
            Page = 0
        };

        paging.SortByPropertyName(nameof(BrandMaintenanceInfo.BrandCode), false);

        var query = new GetAllBrandMaintenanceQuery(
            "",
            paging);

        moqBackOfficeRepository
            .Setup(fake => fake.QueryBrandMaintenanceByBrandCodeAsync(It.Is<string>(x => x == query.BrandCode)))
            .Returns(Array.Empty<BrandMaintenanceInfo>().ToAsyncEnumerable());

        await sut.QueryAsync(query).ToArrayAsync();

        moqBackOfficeRepository.Verify(
            fake => fake.QueryBrandMaintenanceByBrandCodeAsync(It.Is<string>(x => x == query.BrandCode)),
            Times.Exactly(1));
    }

    [Test]
    public async Task User_Can_Update_Brand_Maintenance()
    {
        var moqBackOfficeRepository = new Mock<IBackOfficeRepository>();

        var sut = new BatchUpdateBrandMaintenanceCommandService(moqBackOfficeRepository.Object);

        var command = new BatchUpdateBrandMaintenanceCommand(
            new Guid[] { Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"), Guid.Parse("5dea25ae-431d-4514-aa36-81ab9d81b1dc") },
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        await sut.ExecuteAsync(command);

        moqBackOfficeRepository.Verify(
            fake => fake.BatchUpdateOrAddBrandMaintenanceAsync(
                It.Is<Guid[]>(x => x == command.BrandIds),
                It.Is<DateTimeOffset>(x => x == command.BeginTime),
                It.Is<DateTimeOffset>(x => x == command.EndTime)),
            Times.Exactly(1));
    }

    [Test]
    public async Task User_Can_Update_Brand_Maintenance_Content()
    {
        var moqBackOfficeRepository = new Mock<IBackOfficeRepository>();

        var sut = new UpdateBrandMaintenanceCommandService(moqBackOfficeRepository.Object);

        var id = Guid.NewGuid();
        var content = "This is Content";
        var command = new UpdateBrandMaintenanceCommand(id, content);

        await sut.ExecuteAsync(command);

        moqBackOfficeRepository.Verify(
            fake => fake.UpdateBrandMaintenanceAsync(
                It.Is<Guid>(x => x == command.Id),
                It.Is<string>(x => x == command.Content)),
            Times.Exactly(1));
    }
}
