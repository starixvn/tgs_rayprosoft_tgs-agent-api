using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ExpectedObjects;
using Moq;
using NSubstitute;
using NUnit.Framework;
using TGS.Clients.Promotion.Client;
using TGS.Core.Infrastructures.Json;
using Tgs.FileService.Clients;
using Tgs.Payment.Clients;
using Tgs.Payment.LeePay;
using TGS.Payment.ManualDeposit;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Deposit;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class DepositTests
    {
        [Test]
        public void Bank_Deposit_Form_If_Not_Player_Own_Will_Throw_Exception()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new GetBankDepositFromQueryService(moqPaymentServiceClient.Object);

            var query = new GetBankDepositFormQuery(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid());

            moqPaymentServiceClient
                .Setup(fake => fake.GetDepositFormAsync(
                    It.Is<Guid>(x => x == query.BrandId),
                    It.Is<Guid>(x => x == query.FormId)))
                .Returns(ValueTask.FromResult(new DepositFormInfo
                {
                    Amount = 1000M,
                    BrandId = Guid.NewGuid(),
                    BrandPaymentId = Guid.NewGuid(),
                    ChannelName = "ChannelName",
                    CreatedOn = DateTimeOffset.Now,
                    Id = query.FormId,
                    PaymentMethodId = Guid.NewGuid(),
                    PlayerCode = "OtherPlayerCode",
                    UserName = "OtherUserName",
                    Status = PaymentFlowState.WaitApprove,
                    UpdatedOn = DateTimeOffset.Now
                }));

            Assert.ThrowsAsync<UltraViresException>(async () => await sut.GetAsync(query));
        }

        [Test]
        public async Task Create_Bank_Deposit_Form_Should_Success()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();
            var moqPaymentClient = new Mock<IPaymentClient>();
            var serializer = new ManualDepositSerializer(new JsonSerializationProvider());
            var moqPromotionClient = new Mock<IPromotionClient>();

            var sut = new CreateDepositFormCommandService(
                moqPaymentServiceClient.Object,
                moqPaymentClient.Object,
                moqPromotionClient.Object);

            var command = new CreateDepositFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M,
                "tgs.payment.manual",
                null,
                _ => serializer.Serialize(
                    new ManualDepositApplyInfo(
                        "ThisIsBankAccount",
                        "ThisIsReceiveBankAccount",
                        "ReceiveAccountName")));

            moqPaymentClient
                .Setup(fake => fake.DepositAsync(
                    It.IsAny<string>(),
                    It.Is<decimal>(x => x == command.Amount),
                    It.IsAny<string>()))
                .Returns(ValueTask.FromResult(command.CreatePaymentSettings(Guid.NewGuid())));

            var expected = new DepositFormCreateInfo
            {
                PlayerCode = command.PlayerCode,
                BrandId = command.BrandId,
                BrandPaymentId = command.BrandPaymentId,
                Amount = command.Amount,
                PromotionId = command.PromotionId,
                ApplyInfo = serializer.Serialize(
                    new ManualDepositApplyInfo(
                        "ThisIsBankAccount",
                        "ThisIsReceiveBankAccount",
                        "ReceiveAccountName"))
            }.ToExpectedObject();

            await sut.ExecuteAsync(command);

            moqPaymentServiceClient.Verify(
                fake => fake.CreateDepositFormAsync(
                    It.IsAny<Guid>(),
                    It.Is<DepositFormCreateInfo>(form => expected.Matches(form))),
                Times.Exactly(1));
        }

        [Test]
        public async Task Create_Crypto_Deposit_Form_Should_Success()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();
            var moqPaymentClient = new Mock<IPaymentClient>();
            var serializer = new LeePaySerializer();
            var moqPromotionClient = new Mock<IPromotionClient>();

            var sut = new CreateDepositFormCommandService(
                moqPaymentServiceClient.Object,
                moqPaymentClient.Object,
                moqPromotionClient.Object);

            var createFormId = Guid.NewGuid();
            var command = new CreateDepositFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M,
                "tgs.payment.leepay",
                null,
                _ => serializer.Serialize(
                    new LeePayDepositApplyInfo(
                        new(
                            "systemId",
                            "systemKey",
                            createFormId,
                            "CNY",
                            "http://localhost",
                            null),
                        new(
                            "00",
                            null,
                            "testOrderId",
                            "testOrderNo",
                            createFormId,
                            "walletAddress",
                            "http://test.com",
                            11M,
                            "CNY",
                            0.1M))));

            moqPaymentClient
                .Setup(fake => fake.DepositAsync(
                    It.IsAny<string>(),
                    It.Is<decimal>(x => x == command.Amount),
                    It.IsAny<string>()))
                .Returns(ValueTask.FromResult(command.CreatePaymentSettings(Guid.NewGuid())));

            var expected = new DepositFormCreateInfo
            {
                PlayerCode = command.PlayerCode,
                Amount = command.Amount,
                ApplyInfo = serializer.Serialize(
                    new LeePayDepositApplyInfo(
                        new(
                            "systemId",
                            "systemKey",
                            createFormId,
                            "CNY",
                            "http://localhost",
                            null),
                        new(
                            "00",
                            null,
                            "testOrderId",
                            "testOrderNo",
                            createFormId,
                            "walletAddress",
                            "http://test.com",
                            11M,
                            "CNY",
                            0.1M))),
                BrandId = command.BrandId,
                BrandPaymentId = command.BrandPaymentId,
                PromotionId = command.PromotionId
            }.ToExpectedObject();

            await sut.ExecuteAsync(command);

            moqPaymentServiceClient.Verify(
                fake => fake.CreateDepositFormAsync(
                    It.IsAny<Guid>(),
                    It.Is<DepositFormCreateInfo>(form => expected.Matches(form))),
                Times.Exactly(1));
        }

        [Test]
        public void Daily_Second_Deposit_With_First_Deposit_Promotion_Should_Fail()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();
            var moqPaymentClient = new Mock<IPaymentClient>();
            var moqPromotionClient = new Mock<IPromotionClient>();

            var sut = new CreateDepositFormCommandService(
                moqPaymentServiceClient.Object,
                moqPaymentClient.Object,
                moqPromotionClient.Object);

            var createFormId = Guid.NewGuid();
            var command = new CreateDepositFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M,
                "tgs.payment.leepay",
                Guid.NewGuid(),
                _ => "{}");

            Assert.ThrowsAsync<CannotJoinPromotionException>(async () => await sut.ExecuteAsync(command));
        }

        [Test]
        public async Task Get_Bank_Deposit_Form_By_Id_Should_Success()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();
            var jsonSerializationProvider = new JsonSerializationProvider();

            var sut = new GetBankDepositFromQueryService(moqPaymentServiceClient.Object);

            var query = new GetBankDepositFormQuery(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid());

            moqPaymentServiceClient
                .Setup(fake => fake.GetDepositFormAsync(
                    It.Is<Guid>(x => x == query.BrandId),
                    It.Is<Guid>(x => x == query.FormId)))
                .Returns(ValueTask.FromResult(new DepositFormInfo
                {
                    Amount = 1000M,
                    BrandId = query.BrandId,
                    BrandPaymentId = Guid.NewGuid(),
                    ChannelName = "ChannelName",
                    CreatedOn = DateTimeOffset.Now,
                    Id = query.FormId,
                    PaymentMethodId = Guid.NewGuid(),
                    PlayerCode = query.PlayerCode,
                    Status = PaymentFlowState.WaitApprove,
                    UpdatedOn = DateTimeOffset.Now,
                    UserName = "PlayerName"
                }));

            var actual = await sut.GetAsync(query);

            moqPaymentServiceClient.Verify(
                fake => fake.GetDepositFormAsync(
                    It.Is<Guid>(x => x == query.BrandId),
                    It.Is<Guid>(x => x == query.FormId)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Get_Brand_Payment_Should_SuccessAsync()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();

            var sut = new GetBrandPaymentQueryService(moqPaymentServiceClient.Object);

            var query = new GetBrandPaymentQuery(Guid.NewGuid());

            _ = await sut.GetAsync(query);

            moqPaymentServiceClient.Verify(
                fake => fake.GetBrandPaymentByIdAsync(It.Is<Guid>(x => x == query.Id)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Submit_BankDepositForm_Should_Success()
        {
            var moqPaymentServiceClient = new Mock<IPaymentServiceClient>();
            var moqFileServiceClient = new Mock<IFileServiceClient>();
            var serializer = new ManualDepositSerializer(new JsonSerializationProvider());

            var sut = new SubmitBankDepositFormCommandService(
                moqPaymentServiceClient.Object,
                moqFileServiceClient.Object,
                serializer);

            var playerCode = "ThisIsPlayerCode";
            var formId = Guid.NewGuid();
            var contentType = "image/jpeg";
            var fileStream = new MemoryStream();

            var command = new SubmitBankDepositFormCommand(
                playerCode,
                formId,
                contentType,
                fileStream);

            moqFileServiceClient
                .Setup(fake => fake.UploadFileAsync(
                    It.IsAny<string>(),
                    It.Is<MediaTypeHeaderValue>(x => x.ToString() == contentType),
                    It.IsAny<Stream>()))
                .Returns(ValueTask.FromResult(new FileView
                {
                    Id = "testFileId",
                    ContentType = contentType
                }));

            await sut.ExecuteAsync(command);

            moqFileServiceClient.Verify(
                fake => fake.UploadFileAsync(It.IsAny<string>(), It.IsAny<MediaTypeHeaderValue>(), It.IsAny<Stream>()),
                Times.Exactly(1));
            moqPaymentServiceClient.Verify(
                fake => fake.SubmitDepositFormStatusAsync(
                    It.IsAny<Guid>(),
                    It.Is<PaymentFlowState>(x => x == PaymentFlowState.WaitApprove),
                    It.IsAny<string>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_Get_Deposit_Form()
        {
            var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();

            var sut = new GetDepositFormQueryService(fakePaymentServiceClient);

            var query = new GetDepositFormQuery(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid());

            fakePaymentServiceClient.GetDepositFormAsync(Arg.Is(query.BrandId), Arg.Is(query.FormId))
                .Returns(new DepositFormInfo
                {
                    Amount = 1000M,
                    BrandId = query.BrandId,
                    BrandPaymentId = Guid.NewGuid(),
                    ChannelName = "ChannelName",
                    CreatedOn = DateTimeOffset.Now,
                    Id = query.FormId,
                    PaymentMethodId = Guid.NewGuid(),
                    PlayerCode = query.PlayerCode,
                    Status = PaymentFlowState.WaitApprove,
                    UpdatedOn = DateTimeOffset.Now,
                    UserName = "UserName"
                });

            await sut.GetAsync(query);

            _ = fakePaymentServiceClient.Received(1)
                .GetDepositFormAsync(Arg.Is(query.BrandId), Arg.Is(query.FormId));
        }
    }
}