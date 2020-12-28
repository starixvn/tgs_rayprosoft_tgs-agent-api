using System;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.BrandPayment;
using TGS.Apis.BackendOffice.Controllers.Messages.PaymentChannel;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class PaymentChannelControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new PaymentChannelController(
                _queriesMock.Object,
                _commandsMock.Object);
            _controller.ControllerContext.HttpContext = HttpContextMock.Object;
        }

        private PaymentChannelController _controller;
        private readonly Mock<ITgsQueries<PaymentChannel>> _queriesMock = new();

        private readonly Mock<ITgsCommands<PaymentChannel>> _commandsMock =
            new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_PaymentChannels(bool showAll)
        {
            var request = new PaymentChannelsRequest
            {
                Page = 1,
                Rows = 10,
                SortBy = "updatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var list = CreatePaymentChannelFixture()
                .CreateMany<PaymentChannel>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            var orderedRoles = list.OrderByDescending(p => p.UpdatedOn);

            var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            _queriesMock.Setup(x => x.GetQueries())
                .Returns(list);

            var response = await _controller.GetPaymentChannelsAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].Name.Should().Be(expectedRoles[i].Name);
            }

            _queriesMock.Verify(p => p.GetQueries());
        }

        [Test]
        public async Task Cannot_Create_PaymentChannel_With_Code_Exists()
        {
            var request = new Fixture().Create<CreatePaymentChannelRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentChannel, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.PaymentChannel.CodeExists);
        }

        [Test]
        public async Task Can_Create_PaymentChannel()
        {
            var request = new Fixture().Create<CreatePaymentChannelRequest>();

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentChannel, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.CreateAsync(request);
            response.Valid.Should().BeTrue();

            _queriesMock.Verify(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentChannel, bool>>>()));
            _commandsMock.Verify(p => p.CreateAsync(It.Is<PaymentChannel>(r =>
                r.Code == request.Code)));
        }

        [Test]
        public async Task Can_Update_PaymentChannel()
        {
            var payment = CreatePaymentChannelFixture().Create<PaymentChannel>();
            var request = new Fixture().Create<UpdatePaymentChannelRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(It.IsAny<Expression<Func<PaymentChannel, bool>>>(),
                    It.IsAny<Func<IQueryable<PaymentChannel>, IQueryable<PaymentChannel>>>()))
                .Returns(Task.FromResult(payment));

            _queriesMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<PaymentChannel, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateAsync(payment.Id, request);

            response.Valid.Should().BeTrue();
            _queriesMock.Verify(s => s.GetSingleAsync(It.IsAny<Expression<Func<PaymentChannel, bool>>>(),
                It.IsAny<Func<IQueryable<PaymentChannel>, IQueryable<PaymentChannel>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<PaymentChannel>(r =>
                r.Code == payment.Code)));
        }

        [Test]
        public async Task Can_Update_PaymentChannel_Not_Found()
        {
            var payment = CreatePaymentChannelFixture().Create<PaymentChannel>();
            var request = new Fixture().Create<UpdatePaymentChannelRequest>();

            _queriesMock.Setup(s => s.GetSingleAsync(
                    It.IsAny<Expression<Func<PaymentChannel, bool>>>(),
                    It.IsAny<Func<IQueryable<PaymentChannel>,
                        IQueryable<PaymentChannel>>>()))
                .Returns(Task.FromResult<PaymentChannel>(null));

            _queriesMock.Setup(s =>
                    s.ExistsAsync(It.IsAny<Expression<Func<PaymentChannel, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateAsync(payment.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.PaymentChannel.NotFound);
            _queriesMock.Verify(s => s.GetSingleAsync(
                It.IsAny<Expression<Func<PaymentChannel, bool>>>(),
                It.IsAny<Func<IQueryable<PaymentChannel>,
                    IQueryable<PaymentChannel>>>()));
            _commandsMock.Verify(p => p.UpdateAsync(It.Is<PaymentChannel>(r =>
                r.Code == payment.Code)), Times.Never);
        }

        private Fixture CreatePaymentChannelFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<PaymentChannel>(u => u);
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}