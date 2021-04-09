using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.DataServices.Tgs;
using TGS.Services.BackOffice.Application.Queries.Deposit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TGS.Services.BackOffice.Application.Queries.PaymentMethod;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class PaymentMethodTests
    {
        [Test]
        public async Task User_Can_Get_All_PaymentMethods()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var tgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            await tgsRepository.PaymentMethods.AddAsync(
                new PaymentMethod
                {
                    Id = Guid.NewGuid(),
                    Name = "Test"
                });

            await tgsRepository.SaveChangesAsync();

            var sut = new PaymentMethodQueryService(tgsRepository);

            var query = new PaymentMethodQuery();

            var actual = await sut.QueryAsync(query).ToArrayAsync();

            Assert.IsTrue(actual.Length == 1);
        }
    }
}
