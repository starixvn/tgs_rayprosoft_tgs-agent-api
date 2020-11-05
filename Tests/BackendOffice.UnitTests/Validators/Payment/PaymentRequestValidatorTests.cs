using System;
using FluentValidation.TestHelper;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Apis.BackendOffice.Controllers.Messages.Validators.Payment;

namespace TGS.Tests.BackendOffice.UnitTests.Validators.Payment
{
    [TestFixture]
    public class PaymentRequestValidatorTests
    {
        [SetUp]
        public void Setup()
        {
            _validator = new PaymentRequestValidator();
        }

        private PaymentRequestValidator _validator;

        [Test]
        public void Should_have_error_when_channel_name_is_null()
        {
            var model = new PaymentRequest {ChannelName = null};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.ChannelName)
                .WithErrorMessage(Apis.BackendOffice.Resources.Payment.ChannelNameRequired);
        }

        [Test]
        public void Should_not_have_error_when_channel_name_is_specified()
        {
            var model = new PaymentRequest {ChannelName = "ChannelName"};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.ChannelName);
        }

        [Test]
        public void Should_have_error_when_payment_vendor_id_is_null()
        {
            var model = new PaymentRequest {PaymentVendorId = Guid.Empty};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.PaymentVendorId)
                .WithErrorMessage(Apis.BackendOffice.Resources.Payment.PaymentVendorRequired);
        }

        [Test]
        public void Should_not_have_error_when_payment_vendor_id_is_specified()
        {
            var model = new PaymentRequest {PaymentVendorId = Guid.NewGuid()};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.PaymentVendorId);
        }

        [Test]
        public void Should_have_error_when_payment_method_id_is_null()
        {
            var model = new PaymentRequest {PaymentMethodId = Guid.Empty};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.PaymentMethodId)
                .WithErrorMessage(Apis.BackendOffice.Resources.Payment.PaymentMethodRequired);
        }

        [Test]
        public void Should_not_have_error_when_payment_method_id_is_specified()
        {
            var model = new PaymentRequest {PaymentMethodId = Guid.NewGuid()};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.PaymentMethodId);
        }

        [Test]
        public void Should_have_error_when_payment_channel_id_is_null()
        {
            var model = new PaymentRequest {PaymentChannelId = Guid.Empty};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.PaymentChannelId)
                .WithErrorMessage(Apis.BackendOffice.Resources.Payment.PaymentChannelRequired);
        }

        [Test]
        public void Should_not_have_error_when_payment_channel_id_is_specified()
        {
            var model = new PaymentRequest {PaymentChannelId = Guid.NewGuid()};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.PaymentChannelId);
        }

        [Test]
        public void Should_have_error_when_transfer_limit_to_is_null()
        {
            var model = new PaymentRequest {TransferLimitTo = 0};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.TransferLimitTo)
                .WithErrorMessage(Apis.BackendOffice.Resources.Payment.TransferLimitGreaterZero);
        }

        [Test]
        public void Should_not_have_error_when_transfer_limit_to_is_specified()
        {
            var model = new PaymentRequest {TransferLimitTo = 1};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.TransferLimitTo);
        }

        [Test]
        public void Should_have_error_when_deposited_amount_to_is_null()
        {
            var model = new PaymentRequest {DepositedAmountTo = 0};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.DepositedAmountTo)
                .WithErrorMessage(Apis.BackendOffice.Resources.Payment.DepositedGreaterZero);
        }

        [Test]
        public void Should_not_have_error_when_deposited_amount_to_is_specified()
        {
            var model = new PaymentRequest {DepositedAmountTo = 1};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.DepositedAmountTo);
        }
    }
}