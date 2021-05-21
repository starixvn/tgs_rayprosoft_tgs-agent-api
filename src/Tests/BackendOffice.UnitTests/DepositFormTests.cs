using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using PlayerBucket.Repositories;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Commands.Deposit;
using TGS.Services.BackOffice.Application.Queries.Deposit;
using TGS.Services.BackOffice.Domain;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Payment;
using TGS.Services.WalletService;
using TGS.Services.PlayerBucket.Models;
using TGS.Services.PlayerBucket;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class DepositFormTests
    {
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

        [Test]
        public async Task Manager_Can_Review_DepositForm()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();
            var moqPlayerService = new Mock<IPlayerService>();
            var moqPlayerBucketRepository = new Mock<IPlayerBucketRepository>();

            var sut = new ReviewDepositFormCommandService(
                moqPaymentRepository.Object,
                moqPlayerService.Object,
                moqPlayerBucketRepository.Object);

            var brandId = Guid.NewGuid();
            var formId = Guid.NewGuid();
            var walletId = Guid.NewGuid();
            var command = new ReviewDepositFormCommand(brandId, formId, ReviewState.Approve, null);

            moqPaymentRepository
                .Setup(fake => fake.GetDepositFormByIdAsync(It.Is<Guid>(x => x == formId)))
                .Returns(ValueTask.FromResult(new DepositFormDetail(
                    Guid.NewGuid(),
                    "ThisIsPlayerCode",
                    "PlayerName",
                    brandId,
                    Guid.NewGuid(),
                    "ChannelName",
                    Guid.NewGuid(),
                    1000M,
                    null,
                    1000,
                    null,
                    null,
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow)));
            moqPlayerService
                .Setup(fake => fake.GetPlayerAsync(
                    It.IsAny<Expression<Func<Player, bool>>>(),
                    It.IsAny<Func<IQueryable<Player>, IQueryable<Player>>>()))
                .Returns(Task.FromResult(new Player
                {
                    PlayerCode = "ThisIsPlayerCode",
                    DefaultWalletId = walletId
                }));

            await sut.ExecuteAsync(command);

            moqPaymentRepository.Verify(
                fake => fake.SignOffDepositFormAsync(
                    It.Is<Guid>(x => x == formId),
                    It.Is<ReviewState>(x => x == ReviewState.Approve),
                    It.IsAny<string>()),
                Times.Exactly(1));

            moqPlayerBucketRepository.Verify(
                fake => fake.CreatePlayerBucketWithDepositAsync(
                    It.Is<CreatePlayerBucketWithDepositRequest>(
                        x => x.BrandId == brandId
                            && x.DefaultWalletId == walletId
                            && x.DepositAmount == 1000M
                            && x.PlayerCode == "ThisIsPlayerCode")),
                Times.Exactly(1));
        }

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
                    UpdatedOn = DateTimeOffset.Now
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

            Assert.AreEqual(1, actual.Length);
        }
    }
}
