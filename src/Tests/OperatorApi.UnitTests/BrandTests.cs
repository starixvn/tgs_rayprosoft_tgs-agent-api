using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Brand;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class BrandTests
    {
        [Test]
        public async Task Get_Brand_Should_Success()
        {
            var brandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af");
            var brandCode = "brandCode";
            var fakeSysRepository = Substitute.For<ISysRepository>();

            fakeSysRepository.GetBrandInfoAsync(Arg.Any<Guid>())
                .Returns(new BrandInfo(
                    brandId,
                    brandCode,
                    new Uri("https://img.org/1.png"),
                    new Uri("https://img.org/2.png"),
                    new Uri("https://img.org/3.png"),
                    false));

            var sut = new GetBrandQueryService(fakeSysRepository);

            var query = new BrandQuery(brandId);
            await sut.GetAsync(query);

            _ = fakeSysRepository.Received(1)
                .GetBrandInfoAsync(Arg.Is(brandId));
        }
    }
}