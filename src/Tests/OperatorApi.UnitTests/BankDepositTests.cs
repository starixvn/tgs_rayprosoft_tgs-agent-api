using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ExpectedObjects;
using Moq;
using NUnit.Framework;
using TGS.Apis.FileService.Clients;
using TGS.Services.Operator;
using TGS.Services.Operator.BankDeposit;

namespace TGS.Tests.OperatorApi.UnitTests
{
    [TestFixture]
    public class BankDepositTests
    {
        [Test]
        public async Task Create_Bank_Deposit_Form_Should_Success()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new CreateBankDepositFormCommandService(moqWalletRepository.Object);

            var command = new CreateBankDepositFormCommand(
                "ThisIsPlayerCode",
                Guid.NewGuid(),
                Guid.NewGuid(),
                "ThisIsBankAccount",
                1000M);
            var expected = new BankDepositForm(
                command.PlayerCode,
                command.BrandId,
                command.BrandPaymentId,
                command.BankAccount,
                command.Amount).ToExpectedObject();

            await sut.ExecuteAsync(command);

            moqWalletRepository.Verify(
                fake => fake.CreateDepositFormAsync(It.Is<BankDepositForm>(form => expected.Matches(form))),
                Times.Exactly(1));
        }

        [Test]
        public async Task Get_Bank_Deposit_Form_By_Id_Should_Success()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new GetBankDepositFromQueryService(moqWalletRepository.Object);

            var query = new GetBankDepositFormQuery(
                "ThisIsPlayerCode",
                Guid.NewGuid());

            moqWalletRepository
                .Setup(fake => fake.GetDepositFormByIdAsync(It.Is<Guid>(x => x == query.FormId)))
                .Returns(ValueTask.FromResult(new BankDepositForm(
                    query.PlayerCode,
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "test",
                    1000M)));

            var actual = await sut.GetAsync(query);

            moqWalletRepository.Verify(
                fake => fake.GetDepositFormByIdAsync(It.Is<Guid>(x => x == query.FormId)),
                Times.Exactly(1));
        }

        [Test]
        public void Bank_Deposit_Form_If_Not_Player_Own_Will_Throw_Exception()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new GetBankDepositFromQueryService(moqWalletRepository.Object);

            var query = new GetBankDepositFormQuery(
                "ThisIsPlayerCode",
                Guid.NewGuid());

            moqWalletRepository
                .Setup(fake => fake.GetDepositFormByIdAsync(It.Is<Guid>(x => x == query.FormId)))
                .Returns(ValueTask.FromResult(new BankDepositForm(
                    "OtherPlayerCode",
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "test",
                    1000M)));

            Assert.ThrowsAsync<UltraViresException>(async () => await sut.GetAsync(query));
        }

        [Test]
        public async Task Get_Payment_Methods_Should_Success()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new AllPaymentMethodsQueryService(moqWalletRepository.Object);

            var query = new AllPaymentMethodsQuery();

            moqWalletRepository
                .Setup(fake => fake.GetAllPaymentMethodsAsync())
                .Returns(new[] {
                    new PaymentMethod(Guid.NewGuid(), "test1"),
                    new PaymentMethod(Guid.NewGuid(), "test2")
                }.ToAsyncEnumerable());

            _ = await sut.GetAsync(query);

            moqWalletRepository.Verify(
                fake => fake.GetAllPaymentMethodsAsync(),
                Times.Exactly(1));
        }

        [Test]
        public async Task Get_Payment_Channels_Should_Success()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new AllPaymentChannelsQueryService(moqWalletRepository.Object);

            var query = new AllPaymentChannelsQuery();

            moqWalletRepository
                .Setup(fake => fake.GetAllPaymentChannelsAsync())
                .Returns(new[] {
                    new PaymentChannel(Guid.NewGuid(), "test1"),
                    new PaymentChannel(Guid.NewGuid(), "test2")
                }.ToAsyncEnumerable());

            _ = await sut.GetAsync(query);

            moqWalletRepository.Verify(
                fake => fake.GetAllPaymentChannelsAsync(),
                Times.Exactly(1));
        }

        [Test]
        public async Task Get_Brand_Payment_Should_SuccessAsync()
        {
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new GetBrandPaymentQueryService(moqWalletRepository.Object);

            var query = new GetBrandPaymentQuery(Guid.NewGuid());

            _ = await sut.GetAsync(query);

            moqWalletRepository.Verify(
                fake => fake.GetBrandPaymentByIdAsync(It.Is<Guid>(x => x == query.Id)),
                Times.Exactly(1));
        }

        [Test]
        public async Task Submit_BankDepositForm_Should_Success()
        {
            var moqFileServiceClient = new Mock<IFileServiceClient>();
            var moqWalletRepository = new Mock<IWalletRepository>();

            var sut = new SubmitBankDepositFormCommandService(
                moqFileServiceClient.Object,
                moqWalletRepository.Object);

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
            moqWalletRepository.Verify(
                fake => fake.SubmitRemittanceReceiptToDepositFormAsync(It.IsAny<Guid>(), It.Is<string>(x => x == "testFileId")),
                Times.Exactly(1));
        }
    }
}
