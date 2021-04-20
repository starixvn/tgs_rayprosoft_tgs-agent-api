using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Commands.Deposit;
using TGS.Services.BackOffice.Application.Queries.Deposit;
using TGS.Services.BackOffice.Domain;
using TGS.Services.BackOffice.Domain.Enums;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class DepositFormTests
    {
        [Test]
        public async Task User_Can_List_DepositForm()
        {
            var jsonSerializationProvider = new JsonSerializationProvider();

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

            var pmcId = Guid.NewGuid();
            await tgsRepository.AddAsync(
                new PaymentMethodChannel
                {
                    Id = pmcId,
                    PaymentId = Guid.NewGuid(),
                    MethodId = Guid.NewGuid(),
                    ChannelId = Guid.NewGuid()
                });

            var brandPaymentId = Guid.NewGuid();
            await tgsRepository.BrandPayments.AddAsync(
                new BrandPayment
                {
                    Id = brandPaymentId,
                    BrandId = brandId,
                    PaymentMethodChannelId = pmcId
                });

            await tgsRepository.DepositForms.AddRangeAsync(
                new DepositForm
                {
                    Id = Guid.NewGuid(),
                    PlayerCode = "PlayerCode",
                    BrandId = brandId,
                    Amount = 1000M,
                    BrandPaymentId = brandPaymentId,
                    Status = 1000,
                    ApplyInfo = jsonSerializationProvider.SerializeToString(
                        new
                        {
                            BankAccount = "1234567890"
                        },
                        null),
                    PaymentReturn = null,
                    CreatedOn = DateTimeOffset.Now,
                    UpdateOn = DateTimeOffset.Now
                });

            await tgsRepository.SaveChangesAsync();

            var sut = new DepositFormQueryService(tgsRepository, jsonSerializationProvider);

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

        [Test]
        public async Task Manager_Can_Review_DepositForm()
        {
            var moqDepositRepository = new Mock<IDepositRepository>();

            var sut = new ReviewDepositFormCommandService(moqDepositRepository.Object);

            var formId = Guid.NewGuid();
            var command = new ReviewDepositFormCommand(formId, ReviewStatus.Approve);

            await sut.ExecuteAsync(command);

            moqDepositRepository.Verify(
                fake => fake.SignOffDepositFormAsync(
                    It.Is<Guid>(x => x == formId),
                    It.Is<ReviewStatus>(x => x == ReviewStatus.Approve)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Manager_Can_Get_The_DepositForm_Detail()
        {
            var moqDepositRepository = new Mock<IDepositRepository>();

            var sut = new GetDepositFormDetailQueryService(moqDepositRepository.Object);

            var formId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var query = new GetDepositFormDetailQuery(formId, brandId);

            await sut.GetAsync(query);

            moqDepositRepository.Verify(
                fake => fake.GetDepositFormDetailAsync(It.Is<Guid>(x => x == formId)),
                Times.Exactly(1));
        }
    }
}
