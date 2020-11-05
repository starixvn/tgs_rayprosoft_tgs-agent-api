using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validators.Payment
{
    public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
    {
        public PaymentRequestValidator()
        {
            RuleFor(x => x.ChannelName)
                .NotEmpty()
                .WithMessage(Resources.Payment.ChannelNameRequired);

            RuleFor(x => x.PaymentVendorId)
                .NotEmpty()
                .WithMessage(Resources.Payment.PaymentVendorRequired);

            RuleFor(x => x.PaymentMethodId)
                .NotEmpty()
                .WithMessage(Resources.Payment.PaymentMethodRequired);

            RuleFor(x => x.PaymentChannelId)
                .NotEmpty()
                .WithMessage(Resources.Payment.PaymentChannelRequired);

            RuleFor(x => x.TransferLimitTo)
                .GreaterThan(0)
                .WithMessage(Resources.Payment.TransferLimitGreaterZero);

            RuleFor(x => x.DepositedAmountTo)
                .GreaterThan(0)
                .WithMessage(Resources.Payment.DepositedGreaterZero);
        }
    }
}