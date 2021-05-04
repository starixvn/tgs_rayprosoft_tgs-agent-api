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
using TGS.Services.BackOffice.Application.Commands.Withdraw;
using TGS.Services.BackOffice.Application.Queries.Withdraw;
using TGS.Services.Payment;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class WithdrawFormTests
    {
        [Test]
        public async Task Manager_Can_Get_The_WithdrawForm_Detail()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new GetWithdrawFormDetailQueryService(moqPaymentRepository.Object);

            var formId = Guid.NewGuid();
            var brandId = Guid.NewGuid();

            var query = new GetWithdrawFormDetailQuery(formId, brandId);

            await sut.GetAsync(query);

            moqPaymentRepository.Verify(
                fake => fake.GetWithdrawFormByIdAsync(It.Is<Guid>(x => x == formId)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Manager_Can_Review_WithdrawForm()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new ReviewWithdrawFormCommandService(moqPaymentRepository.Object);

            var formId = Guid.NewGuid();
            var command = new ReviewWithdrawFormCommand(formId, ReviewStatus.Approve, null);

            await sut.ExecuteAsync(command);

            moqPaymentRepository.Verify(
                fake => fake.SignOffWithdrawFormAsync(
                    It.Is<Guid>(x => x == formId),
                    It.Is<ReviewStatus>(x => x == ReviewStatus.Approve),
                    It.IsAny<string>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_List_WithdrawFormAsync()
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

            await tgsRepository.WithdrawForms.AddRangeAsync(
                new WithdrawForm
                {
                    Id = Guid.NewGuid(),
                    PlayerCode = "PlayerCode",
                    BrandId = brandId,
                    Amount = 1000M,
                    Status = 1000,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedOn = DateTimeOffset.Now
                });

            await tgsRepository.SaveChangesAsync();

            var sut = new WithdrawFormQueryService(tgsRepository, jsonSerializationProvider);

            var query = new WithdrawFormQuery(
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
