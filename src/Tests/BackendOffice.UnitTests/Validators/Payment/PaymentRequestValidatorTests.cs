using System;
using FluentValidation.TestHelper;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment;
using TGS.Apis.BackendOffice.Controllers.Messages.Validators.BrandPayment;
using TGS.Apis.BackendOffice.Resources;

namespace TGS.Tests.BackendOffice.UnitTests.Validators.Payment
{
    [TestFixture]
    public class BrandPaymentRequestValidatorTests
    {
        [SetUp]
        public void Setup()
        {
            _validator = new BrandPaymentRequestValidator();
        }

        private BrandPaymentRequestValidator _validator;

        [Test]
        public void Should_have_error_when_channel_name_is_null()
        {
            var model = new BrandPaymentRequest {ChannelName = null};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.ChannelName)
                .WithErrorMessage(BrandPayment.ChannelNameRequired);
        }

        [Test]
        public void Should_not_have_error_when_channel_name_is_specified()
        {
            var model = new BrandPaymentRequest {ChannelName = "ChannelName"};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.ChannelName);
        }

        [Test]
        public void Should_have_error_when_payment_vendor_id_is_null()
        {
            var model = new BrandPaymentRequest {PaymentId = Guid.Empty};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.PaymentId)
                .WithErrorMessage(BrandPayment.PaymentRequired);
        }

        [Test]
        public void Should_not_have_error_when_payment_vendor_id_is_specified()
        {
            var model = new BrandPaymentRequest {PaymentId = Guid.NewGuid()};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.PaymentId);
        }

        [Test]
        public void Should_have_error_when_payment_method_id_is_null()
        {
            var model = new BrandPaymentRequest {PaymentMethodId = Guid.Empty};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.PaymentMethodId)
                .WithErrorMessage(BrandPayment.PaymentMethodRequired);
        }

        [Test]
        public void Should_not_have_error_when_payment_method_id_is_specified()
        {
            var model = new BrandPaymentRequest {PaymentMethodId = Guid.NewGuid()};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.PaymentMethodId);
        }

        [Test]
        public void Should_have_error_when_payment_channel_id_is_null()
        {
            var model = new BrandPaymentRequest {PaymentChannelId = Guid.Empty};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.PaymentChannelId)
                .WithErrorMessage(BrandPayment.PaymentChannelRequired);
        }

        [Test]
        public void Should_not_have_error_when_payment_channel_id_is_specified()
        {
            var model = new BrandPaymentRequest {PaymentChannelId = Guid.NewGuid()};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.PaymentChannelId);
        }

        [Test]
        public void Should_have_error_when_transfer_limit_to_is_null()
        {
            var model = new BrandPaymentRequest {TransferLimitTo = 0};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.TransferLimitTo)
                .WithErrorMessage(BrandPayment.TransferLimitGreaterZero);
        }

        [Test]
        public void Should_not_have_error_when_transfer_limit_to_is_specified()
        {
            var model = new BrandPaymentRequest {TransferLimitTo = 1};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.TransferLimitTo);
        }

        [Test]
        public void Should_have_error_when_deposited_amount_to_is_null()
        {
            var model = new BrandPaymentRequest {DepositedAmountMax = 0};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.DepositedAmountMax)
                .WithErrorMessage(BrandPayment.DepositedGreaterZero);
        }

        [Test]
        public void Should_not_have_error_when_deposited_amount_to_is_specified()
        {
            var model = new BrandPaymentRequest {DepositedAmountMax = 1};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.DepositedAmountMax);
        }
    }
}