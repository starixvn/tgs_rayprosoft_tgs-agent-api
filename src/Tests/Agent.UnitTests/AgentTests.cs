using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Agent.Services.Agent;

using CoreModels = TGS.Core.DataServices.Tgs.Models;

namespace Agent.UnitTests
{
    [TestFixture]
    public class AgentTests
    {
        [Test]
        public async Task Create_Agent_Should_Success()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            ITgsRepository repository = new TgsRepository(dbContextOptionsBuilder.Options);

            var brandId = Guid.NewGuid();
            var brand = new Brand()
            {
                Id = brandId,
                Currencies = new List<BrandCurrency>()
                    {
                        new BrandCurrency()
                        {
                            BrandId=brandId,
                            CurrencyCode="CNY"
                        }
                    }
            };

            await repository.Brands.AddAsync(brand);
            var parentAgentId = Guid.NewGuid();
            var parentAgent = new CoreModels.Agent()
            {
                Id = parentAgentId
            };
            await repository.Agents.AddAsync(parentAgent);
            await repository.SaveChangesAsync();

            var fakePasswordHandler = Substitute.For<IPasswordHandler>();

            var sut = new GetCreateAgentQueryService(repository, fakePasswordHandler);

            var query = new GetCreateAgentQuery()
            {
                ParentAgentId = parentAgentId,
                BrandId = brandId,
                UserName = "UserName",
                Password = "Password",
                Remarks = "Remarks"
            };

            var info = await sut.GetAsync(query);

            Assert.IsTrue(info.IsNew);
        }
    }
}