using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Queries.Deposit;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class DepositFormTests
    {
        [Test]
        public async Task User_Can_List_DepositForm()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsRepository>();
            dbContextOptionsBuilder
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var tgsRepository = new TgsRepository(dbContextOptionsBuilder.Options);

            var brandId = Guid.NewGuid();

            await tgsRepository.Players.AddAsync(
                new Player
                {
                    PlayerCode = "PlayerCode",
                    BrandId = brandId
                });

            var brandPaymentId = Guid.NewGuid();

            await tgsRepository.BrandPayments.AddAsync(
                new BrandPayment
                {
                    Id = brandPaymentId,
                    BrandId = brandId
                });

            await tgsRepository.DepositForms.AddRangeAsync(
                new DepositForm
                {
                    Id = Guid.NewGuid(),
                    PlayerCode = "PlayerCode",
                    BrandId = brandId,
                    BankAccount = "123456789",
                    Amount = 1000M,
                    BrandPaymentId = brandPaymentId,
                    Status = 1000,
                    CreatedOn = DateTimeOffset.Now,
                    UpdateOn = DateTimeOffset.Now
                });

            await tgsRepository.SaveChangesAsync();

            var sut = new DepositFormQueryService(tgsRepository);

            var query = new DepositFormQuery(
                brandId,
                null,
                null,
                null,
                null,
                null,
                null,
                new PageableRequest());

            var actual = await sut.QueryAsync(query).ToArrayAsync();

            Assert.IsTrue(actual.Length == 1);
        }
    }
}
