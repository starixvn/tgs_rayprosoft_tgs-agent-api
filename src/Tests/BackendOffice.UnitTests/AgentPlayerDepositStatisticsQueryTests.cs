using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Queries.ReportPlayerDepositQueries;
using TGS.Services.BackOffice.Domain;

namespace TGS.Tests.BackendOffice.UnitTests;

public class AgentPlayerDepositStatisticsQueryTests
{
    [Test]
    public async Task Query_Data()
    {
        var fakePaymentReportingRepository = Substitute.For<IPaymentReportingRepository>();

        var sut = new AgentPlayerDepositQueryService(fakePaymentReportingRepository);

        var query = new AgentPlayerDepositQuery(Guid.NewGuid());

        await sut.QueryAsync(query).ToArrayAsync();

        _ = fakePaymentReportingRepository.Received(1)
            .QueryAgentDailyDepositStatisticsAsync(Arg.Is(query.BrandId), Arg.Any<DateOnly>());
    }
}
