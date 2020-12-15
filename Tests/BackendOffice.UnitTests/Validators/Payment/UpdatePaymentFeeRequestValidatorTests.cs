using FluentValidation.TestHelper;
using NUnit.Framework;
using TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment;
using TGS.Apis.BackendOffice.Controllers.Messages.Validators.BrandPayment;

namespace TGS.Tests.BackendOffice.UnitTests.Validators.Payment
{
    [TestFixture]
    public class UpdatePaymentFeeRequestValidatorTests
    {
        [SetUp]
        public void Setup()
        {
            _validator = new UpdatePaymentFeeRequestValidator();
        }

        private UpdatePaymentFeeRequestValidator _validator;

        [Test]
        public void Should_have_error_when_fee_percentage_is_less_than_zero()
        {
            var model = new UpdatePaymentFeeRequest {FeePercentage = -1};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.FeePercentage)
                .WithErrorMessage(Apis.BackendOffice.Resources.BrandPayment.FeePercentageGreaterEqualZero);
        }

        [Test]
        public void Should_not_have_error_when_fee_percentage_is_not_less_than_zero()
        {
            var model = new UpdatePaymentFeeRequest {LowestFee = 0};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.LowestFee);
        }

        [Test]
        public void Should_have_error_when_lowest_fee_is_less_than_zero()
        {
            var model = new UpdatePaymentFeeRequest {LowestFee = -1};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request.LowestFee)
                .WithErrorMessage(Apis.BackendOffice.Resources.BrandPayment.LowestFeeGreaterEqualZero);
        }

        [Test]
        public void Should_not_have_error_when_lowest_fee_is_not_less_than_zero()
        {
            var model = new UpdatePaymentFeeRequest {LowestFee = 0};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request.LowestFee);
        }

        [Test]
        public void Should_have_error_when_highest_fee_is_less_than_lowest_fee()
        {
            var model = new UpdatePaymentFeeRequest {LowestFee = 5, HighestFee = 1};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(request => request)
                .WithErrorMessage(Apis.BackendOffice.Resources.BrandPayment.HighestFeeGreaterLowestFee);
        }

        [Test]
        public void Should_not_have_error_when_lowest_fee_is_less_than_highest_fee()
        {
            var model = new UpdatePaymentFeeRequest {LowestFee = 0, HighestFee = 1};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(request => request);
        }
    }
}