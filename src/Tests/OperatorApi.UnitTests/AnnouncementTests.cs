using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.Operator;
using TGS.Services.Operator.ApplicationServices.Announcement;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class AnnouncementTests
    {
        [Test]
        public async Task Can_Get_Announcements()
        {
            var fakeSysRepository = Substitute.For<ISysRepository>();

            var sut = new GetAnnouncementQueryService(fakeSysRepository);

            var query = new GetAnnouncementQuery(Guid.NewGuid());

            await sut.QueryAsync(query).ToArrayAsync();

            fakeSysRepository
                .Received(1)
                .QueryAnnouncementInfosAsync(
                    Arg.Is(query.BrandId),
                    Arg.Any<DateTimeOffset>());
        }
    }
}
