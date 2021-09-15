using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operator.Services.Brand;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class BrandTests
    {
        [Test]
        public async Task Get_Brand_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);
            var brandId = new Guid("4e86719f-243f-46f6-b010-5c758b7285af");

            repository.Brands.Add(new Brand
            {
                Id = brandId,
                LogoInfo =
                    "{\"BrandId\": \"4e86719f-243f-46f6-b010-5c758b7285af\", \"LoadingUrl\": \"http://LoadingIconUrl\", \"MobileLogoUrl\": \"http://MobileLogoUrl\",\"DesktopLogoUrl\": \"http://DesktopLogoUrl\"}"
            });
            await repository.SaveChangesAsync();

            var sut = new GetBrandQueryService(repository);

            var query = new BrandQuery(brandId);
            var info = await sut.GetAsync(query).ConfigureAwait(false);

            Assert.IsTrue(info != null);
        }
    }
}