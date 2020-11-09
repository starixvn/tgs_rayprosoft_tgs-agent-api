using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validators.Payment
{
    public class UpdatePaymentFeeRequestValidator : AbstractValidator<UpdatePaymentFeeRequest>
    {
        public UpdatePaymentFeeRequestValidator()
        {
            RuleFor(x => x.FeePercentage)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.Payment.FeePercentageGreaterEqualZero);
            RuleFor(x => x.LowestFee)
                .GreaterThanOrEqualTo(0)
                .WithMessage(Resources.Payment.LowestFeeGreaterEqualZero);
            RuleFor(x => x)
                .Custom((request, context) =>
                {
                    if (request.LowestFee > request.HighestFee)
                        context.AddFailure(Resources.Payment.HighestFeeGreaterLowestFee);
                });
        }
    }
}