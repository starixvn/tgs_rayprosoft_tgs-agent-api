using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validators.BrandPayment
{
    public class UpdatePaymentFeeRequestValidator : AbstractValidator<UpdatePaymentFeeRequest>
    {
        public UpdatePaymentFeeRequestValidator()
        {
            RuleFor(x => x.FeePercentage)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.BrandPayment.FeePercentageGreaterEqualZero);
            RuleFor(x => x.LowestFee)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.BrandPayment.LowestFeeGreaterEqualZero);
            RuleFor(x => x)
                .Custom((request, context) =>
                {
                    if (request.LowestFee > request.HighestFee)
                    {
                        context.AddFailure(Resources.BrandPayment.HighestFeeGreaterLowestFee);
                    }
                });
        }
    }
}