using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ExpectedObjects;
using Moq;
using NSubstitute;
using NUnit.Framework;
using Tgs.Payment;
using Tgs.Payment.LeePay;
using TGS.Apis.FileService.Clients;
using TGS.Clients.Promotion.Client;
using TGS.Core.Infrastructures.Json;
using TGS.Payment.ManualDeposit;
using TGS.Services.Operator;
using TGS.Services.Payment;
using TGS.Services.Operator.Services.Deposit;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class DepositTests
    {
        [Test]
        public void Bank_Deposit_Form_If_Not_Player_Own_Will_Throw_Exception()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new GetBankDepositFromQueryService(moqPaymentRepository.Object);

            var query = new GetBankDepositFormQuery(
                "ThisIsPlayerCode",
                Guid.NewGuid());

            moqPaymentRepository
                .Setup(fake => fake.GetDepositFormByIdAsync(It.Is<Guid>(x => x == query.FormId)))
                .Returns(ValueTask.FromResult(new DepositFormDetail(
                    Guid.NewGuid(),
                    "OtherPlayerCode",
                    "OtherPlayerName",
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "ChannelName",
                    Guid.NewGuid(),
                    1000M,
                    null,
                    1000,
                    null,
                    null,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now)));

            Assert.ThrowsAsync<UltraViresException>(async () => await sut.GetAsync(query));
        }

        [Test]
        public async Task Create_Bank_Deposit_Form_Should_Success()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();
            var moqPaymentClient = new Mock<IPaymentClient>();
            var serializer = new ManualDepositSerializer(new JsonSerializationProvider());
            var moqPromotionClient = new Mock<IPromotionClient>();

            var sut = new CreateDepositFormCommandService(
                moqPaymentRepository.Object,
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

            var expected = new DepositFormInfo(
                command.PlayerCode,
                command.BrandId,
                command.BrandPaymentId,
                command.Amount,
                serializer.Serialize(
                    new ManualDepositApplyInfo(
                        "ThisIsBankAccount",
                        "ThisIsReceiveBankAccount",
                        "ReceiveAccountName"))).ToExpectedObject();

            await sut.ExecuteAsync(command);

            moqPaymentRepository.Verify(
                fake => fake.CreateDepositFormAsync(
                    It.IsAny<Guid>(),
                    It.Is<DepositFormInfo>(form => expected.Matches(form))),
                Times.Exactly(1));
        }

        [Test]
        public async Task Create_Crypto_Deposit_Form_Should_Success()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();
            var moqPaymentClient = new Mock<IPaymentClient>();
            var serializer = new LeePaySerializer(new JsonSerializationProvider());
            var moqPromotionClient = new Mock<IPromotionClient>();

            var sut = new CreateDepositFormCommandService(
                moqPaymentRepository.Object,
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
                            new LeePayDepositSettings(
                                "systemId",
                                "systemKey",
                                createFormId,
                                "CNY",
                                "http://localhost",
                                null),
                            new LeePayDepositResponse(
                                "00",
                                null,
                                "testOrderId",
                                "testOrderNo",
                                createFormId,
                                "walletAddress",
                                "http://test.com",
                                11M,
                                "CNY",
                                0.1M,
                                null))));

            moqPaymentClient
                .Setup(fake => fake.DepositAsync(
                    It.IsAny<string>(),
                    It.Is<decimal>(x => x == command.Amount),
                    It.IsAny<string>()))
                .Returns(ValueTask.FromResult(command.CreatePaymentSettings(Guid.NewGuid())));

            var expected = new DepositFormInfo(
                command.PlayerCode,
                command.BrandId,
                command.BrandPaymentId,
                command.Amount,
                serializer.Serialize(
                    new LeePayDepositApplyInfo(
                            new LeePayDepositSettings(
                                "systemId",
                                "systemKey",
                                createFormId,
                                "CNY",
                                "http://localhost",
                                null),
                            new LeePayDepositResponse(
                                "00",
                                null,
                                "testOrderId",
                                "testOrderNo",
                                createFormId,
                                "walletAddress",
                                "http://test.com",
                                11M,
                                "CNY",
                                0.1M,
                                null)))).ToExpectedObject();

            await sut.ExecuteAsync(command);

            moqPaymentRepository.Verify(
                fake => fake.CreateDepositFormAsync(
                    It.IsAny<Guid>(),
                    It.Is<DepositFormInfo>(form => expected.Matches(form))),
                Times.Exactly(1));
        }

        [Test]
        public async Task Get_Bank_Deposit_Form_By_Id_Should_Success()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();
            var jsonSerializationProvider = new JsonSerializationProvider();

            var sut = new GetBankDepositFromQueryService(moqPaymentRepository.Object);

            var query = new GetBankDepositFormQuery(
                "ThisIsPlayerCode",
                Guid.NewGuid());

            moqPaymentRepository
                .Setup(fake => fake.GetDepositFormByIdAsync(It.Is<Guid>(x => x == query.FormId)))
                .Returns(ValueTask.FromResult(new DepositFormDetail(
                    Guid.NewGuid(),
                    query.PlayerCode,
                    "PlayerName",
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "ChannelName",
                    Guid.NewGuid(),
                    1000M,
                    null,
                    1000,
                    null,
                    null,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now)));

            var actual = await sut.GetAsync(query);

            moqPaymentRepository.Verify(
                fake => fake.GetDepositFormByIdAsync(It.Is<Guid>(x => x == query.FormId)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Get_Brand_Payment_Should_SuccessAsync()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new GetBrandPaymentQueryService(moqPaymentRepository.Object);

            var query = new GetBrandPaymentQuery(Guid.NewGuid());

            _ = await sut.GetAsync(query);

            moqPaymentRepository.Verify(
                fake => fake.GetBrandPaymentByIdAsync(It.Is<Guid>(x => x == query.Id)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Submit_BankDepositForm_Should_Success()
        {
            var moqFileServiceClient = new Mock<IFileServiceClient>();
            var moqPaymentRepository = new Mock<IPaymentRepository>();
            var serializer = new ManualDepositSerializer(new JsonSerializationProvider());

            var sut = new SubmitBankDepositFormCommandService(
                moqFileServiceClient.Object,
                moqPaymentRepository.Object,
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
            moqPaymentRepository.Verify(
                fake => fake.SubmitDepositFormStatusAsync(
                    It.IsAny<Guid>(),
                    It.Is<PaymentFlowState>(x => x == PaymentFlowState.WaitApprove),
                    It.IsAny<string>()),
                Times.Exactly(1));
        }

        [Test]
        public async Task User_Can_Get_Deposit_Form()
        {
            var fakePaymentRepository = Substitute.For<IPaymentRepository>();

            var sut = new GetDepositFormQueryService(fakePaymentRepository);

            var query = new GetDepositFormQuery("ThisIsPlayerCode", Guid.NewGuid());

            fakePaymentRepository
                .GetDepositFormByIdAsync(Arg.Is(query.FormId))
                .Returns(new DepositFormDetail(
                    Guid.NewGuid(),
                    query.PlayerCode,
                    "UserName",
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "ChannelName",
                    Guid.NewGuid(),
                    1000M,
                    null,
                    1000,
                    null,
                    null,
                    DateTimeOffset.Now,
                    DateTimeOffset.Now));

            await sut.GetAsync(query);

            _ = fakePaymentRepository
                .Received(1)
                .GetDepositFormByIdAsync(Arg.Is(query.FormId));
        }
    }
}
