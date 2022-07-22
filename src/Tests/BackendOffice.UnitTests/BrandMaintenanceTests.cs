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

        var sut = new UpdateBrandMaintenanceCommandService(moqBackOfficeRepository.Object);

        var command = new BatchUpdateBrandMaintenanceCommand(
            new string[] { "GPI", "GPI_PHP" },
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        await sut.ExecuteAsync(command);

        moqBackOfficeRepository.Verify(
            fake => fake.BatchUpdateOrAddBrandMaintenanceAsync(
                It.Is<string[]>(x => x == command.BrandCodes),
                It.Is<DateTimeOffset>(x => x == command.BeginTime),
                It.Is<DateTimeOffset>(x => x == command.EndTime)),
            Times.Exactly(1));
    }
}
