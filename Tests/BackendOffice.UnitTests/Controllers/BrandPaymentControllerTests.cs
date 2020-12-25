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
using TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment;
using TGS.Apis.BackendOffice.DataService.Services;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;

namespace TGS.Tests.BackendOffice.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class BrandPaymentControllerTests : BaseTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _controller = new BrandPaymentController(
                _playerQueriesMock.Object,
                _uploadServiceMock.Object,
                _paymentServiceMock.Object,
                _auditLogQueriesMock.Object);
            _controller.ControllerContext.HttpContext = HttpContextMock.Object;
        }

        private BrandPaymentController _controller;
        private readonly Mock<IBrandPaymentService> _paymentServiceMock = new();

        private readonly Mock<ITgsQueries<PaymentAuditLog>> _auditLogQueriesMock =
            new();

        private readonly Mock<ITgsQueries<Player>> _playerQueriesMock =
            new();

        private readonly Mock<IUploadService> _uploadServiceMock = new();

        [TestCase(true)]
        [TestCase(false)]
        public async Task Can_Get_Payments(bool showAll)
        {
            var request = new GetBrandPaymentsRequest
            {
                PaymentStatus = SwitchStatus.Enabled,
                Page = 1,
                Rows = 10,
                SortBy = "updatedOn",
                OrderBy = SortOrder.Descending,
                ShowAll = showAll
            };

            var list = CreatePaymentFixture()
                .CreateMany<BrandPayment>(105)
                .AsQueryable()
                .BuildMock().Object;

            var (skip, take) = GetPage(request);
            var orderedRoles = list.Where(x => x.PaymentStatus == SwitchStatus.Enabled)
                .OrderByDescending(p => p.UpdatedOn);

            var expectedRoles = showAll ? orderedRoles.ToArray() : orderedRoles.Skip(skip).Take(take).ToArray();

            _paymentServiceMock.Setup(x => x.GetPaymentsAsync(false))
                .Returns(list);

            var response = await _controller.GetBrandPaymentsAsync(request);
            response.Valid.Should().BeTrue();

            for (var i = 0; i < response.Items.Length; i++)
            {
                response.Items[i].Id.Should().Be(expectedRoles[i].Id);
                response.Items[i].ChannelName.Should().Be(expectedRoles[i].ChannelName);
            }

            _paymentServiceMock.Verify(p => p.GetPaymentsAsync(false));
        }

        [Test]
        public async Task Cannot_Create_Payment_With_Name_Exists()
        {
            var request = new Fixture().Create<CreateBrandPaymentRequest>();

            _paymentServiceMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<BrandPayment, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.CreateBrandPaymentAsync(request);
            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.BrandPayment.NameExists);
        }

        [Test]
        public async Task Cannot_Create_Payment_With_Whitelist_Same_As_Blacklist()
        {
            var request = new Fixture().Create<CreateBrandPaymentRequest>();
            request.Whitelist[0] = request.Blacklist[0];
            var response = await _controller.CreateBrandPaymentAsync(request);
            response.Valid.Should().BeFalse();
            response.DuplicatePlayers[0].Should().Be(request.Whitelist[0]);
            response.Message.Should().Be(Apis.BackendOffice.Resources.BrandPayment.WhiteSameAsBlack);
        }

        [Test]
        public async Task Can_Create_Payment()
        {
            var request = new Fixture().Create<CreateBrandPaymentRequest>();

            _paymentServiceMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<BrandPayment, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.CreateBrandPaymentAsync(request);
            response.Valid.Should().BeTrue();

            _paymentServiceMock.Verify(s => s.ExistsAsync(It.IsAny<Expression<Func<BrandPayment, bool>>>()));
            _paymentServiceMock.Verify(p => p.CreatePaymentAsync(It.Is<CreateBrandPaymentRequest>(r =>
                    r.ChannelName == request.ChannelName),
                It.Is<string>(r => r == Username)));
        }

        [Test]
        public async Task Can_Update_Payment()
        {
            var payment = CreatePaymentFixture().Create<BrandPayment>();
            var request = new Fixture().Create<UpdateBrandPaymentRequest>();

            _paymentServiceMock.Setup(s => s.GetPaymentAsync(It.IsAny<Expression<Func<BrandPayment, bool>>>(),
                    It.IsAny<Func<IQueryable<BrandPayment>, IQueryable<BrandPayment>>>(), true))
                .Returns(Task.FromResult(payment));

            _paymentServiceMock.Setup(s => s.ExistsAsync(It.IsAny<Expression<Func<BrandPayment, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateBrandPaymentAsync(payment.Id, request);

            response.Valid.Should().BeTrue();
            _paymentServiceMock.Verify(s => s.GetPaymentAsync(It.IsAny<Expression<Func<BrandPayment, bool>>>(),
                It.IsAny<Func<IQueryable<BrandPayment>, IQueryable<BrandPayment>>>(), true));
            _paymentServiceMock.Verify(p => p.UpdatePaymentAsync(It.Is<BrandPayment>(r =>
                    r.ChannelName == payment.ChannelName),
                It.IsAny<UpdateBrandPaymentRequest>(),
                It.Is<string>(r => r == Username)));
        }

        [Test]
        public async Task Cannot_Update_Payment_When_Not_Found()
        {
            var payment = CreatePaymentFixture().Create<BrandPayment>();
            var request = new Fixture().Create<UpdateBrandPaymentRequest>();

            _paymentServiceMock.Setup(s => s.GetPaymentAsync(
                    It.IsAny<Expression<Func<BrandPayment, bool>>>(),
                    It.IsAny<Func<IQueryable<BrandPayment>,
                        IQueryable<BrandPayment>>>(), true))
                .Returns(Task.FromResult<BrandPayment>(null));

            _paymentServiceMock.Setup(s =>
                    s.ExistsAsync(It.IsAny<Expression<Func<BrandPayment, bool>>>()))
                .Returns(Task.FromResult(false));

            var response = await _controller.UpdateBrandPaymentAsync(payment.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.BrandPayment.NotFound);
            _paymentServiceMock.Verify(s => s.GetPaymentAsync(
                It.IsAny<Expression<Func<BrandPayment, bool>>>(),
                It.IsAny<Func<IQueryable<BrandPayment>,
                    IQueryable<BrandPayment>>>(), true));
            _paymentServiceMock.Verify(p => p.UpdatePaymentAsync(It.Is<BrandPayment>(r =>
                    r.ChannelName == payment.ChannelName),
                It.IsAny<UpdateBrandPaymentRequest>(),
                It.Is<string>(r => r == Username)), Times.Never);
        }

        [Test]
        public async Task Cannot_Update_Payment_With_Name_Exists()
        {
            var payment = CreatePaymentFixture().Create<BrandPayment>();
            var request = new Fixture().Create<UpdateBrandPaymentRequest>();
            request.ChannelName = payment.ChannelName;
            _paymentServiceMock.Setup(s => s.GetPaymentAsync(
                    It.IsAny<Expression<Func<BrandPayment, bool>>>(),
                    It.IsAny<Func<IQueryable<BrandPayment>,
                        IQueryable<BrandPayment>>>(), true))
                .Returns(Task.FromResult(payment));

            _paymentServiceMock.Setup(s =>
                    s.ExistsAsync(It.IsAny<Expression<Func<BrandPayment, bool>>>()))
                .Returns(Task.FromResult(true));

            var response = await _controller.UpdateBrandPaymentAsync(payment.Id, request);

            response.Valid.Should().BeFalse();
            response.Message.Should().Be(Apis.BackendOffice.Resources.BrandPayment.NameExists);
            _paymentServiceMock.Verify(s => s.GetPaymentAsync(
                It.IsAny<Expression<Func<BrandPayment, bool>>>(),
                It.IsAny<Func<IQueryable<BrandPayment>,
                    IQueryable<BrandPayment>>>(), true));
            _paymentServiceMock.Verify(p => p.UpdatePaymentAsync(It.Is<BrandPayment>(r =>
                    r.ChannelName == payment.ChannelName),
                It.IsAny<UpdateBrandPaymentRequest>(),
                It.Is<string>(r => r == Username)), Times.Never);
        }

        private Fixture CreatePaymentFixture()
        {
            var fixture = new Fixture();
            fixture.Customize<BrandPayment>(u => u
                .With(s => s.PaymentPlayerList, new PaymentPlayerList[] { })
                .With(s => s.VipLevels, new PaymentVipLevel[] { }));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}