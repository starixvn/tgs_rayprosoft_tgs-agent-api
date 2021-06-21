using System;
using Moq;
using NUnit.Framework;
using Tgs.Payment;
using TGS.Services.BackOffice.Application.Commands.PaymentMethodChannel;
using TGS.Services.BackOffice.Application.Queries.BrandPayment;
using TGS.Services.BackOffice.Domain;

namespace TGS.Tests.BackendOffice.UnitTests
{
    [TestFixture]
    public class PaymentMethodChannelTests
    {
        [Test]
        public void Can_Create_PaymentMethodChannel_Data()
        {
            var moqSystemPaymentRepository = new Mock<ISystemPaymentRepository>();

            var sut = new CreatePaymentMethodChannelCommandService(moqSystemPaymentRepository.Object);

            var command = new CreatePaymentMethodChannelCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid());

            sut.ExecuteAsync(command);

            moqSystemPaymentRepository
                .Verify(
                    fake => fake.CreatePaymentMethodChannelAsync(
                        It.Is<Guid>(x => x == command.PaymentId),
                        It.Is<Guid>(x => x == command.MethodId),
                        It.Is<Guid>(x => x == command.ChannelId)),
                    Times.Exactly(1));
        }

        [Test]
        public void Can_Delete_PaymentMethodChannel_Data()
        {
            var moqSystemPaymentRepository = new Mock<ISystemPaymentRepository>();

            var sut = new DeletePaymentMethodChannelCommandService(moqSystemPaymentRepository.Object);

            var command = new DeletePaymentMethodChannelCommand(Guid.NewGuid());

            sut.ExecuteAsync(command);

            moqSystemPaymentRepository
                .Verify(
                    fake => fake.DeletePaymentMethodChannelAsync(
                        It.Is<Guid>(x => x == command.Id)),
                    Times.Exactly(1));
        }

        [Test]
        public void Should_Get_All_PaymentMethodChannels()
        {
            var moqPaymentRepository = new Mock<IPaymentRepository>();

            var sut = new AllPaymentMethodChannelQueryService(moqPaymentRepository.Object);

            sut.QueryAsync(new AllPaymentMethodChannelQuery());

            moqPaymentRepository.Verify(
                fake => fake.GetPaymentMethodChannelsAsync(),
                Times.Exactly(1));
        }
    }
}
