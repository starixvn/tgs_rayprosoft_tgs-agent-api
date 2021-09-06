using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class BrandLogoTests
    {
        [Test]
        public async Task Can_Get_Brand_LogoInfo()
        {
            var moqBackOfficeRepository = new Mock<IBackOfficeRepository>();

            var sut = new GetBrandLogoQueryService(moqBackOfficeRepository.Object);

            var query = new GetBrandLogoQuery(Guid.NewGuid());

            await sut.GetAsync(query);

            moqBackOfficeRepository.Verify(
                fake => fake.GetBrandLogoInfoAsync(It.Is<Guid>(x => x == query.BrandId)),
                Times.Exactly(1));
        }
    }
}
