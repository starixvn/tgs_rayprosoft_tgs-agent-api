using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validators.BrandPayment
{
    public class BrandPaymentRequestValidator : AbstractValidator<BrandPaymentRequest>
    {
        public BrandPaymentRequestValidator()
        {
            RuleFor(x => x.ChannelName)
                .NotEmpty()
                .WithMessage(Resources.BrandPayment.ChannelNameRequired);

            RuleFor(x => x.PaymentVendorId)
                .NotEmpty()
                .WithMessage(Resources.BrandPayment.PaymentVendorRequired);

            RuleFor(x => x.PaymentMethodId)
                .NotEmpty()
                .WithMessage(Resources.BrandPayment.PaymentMethodRequired);

            RuleFor(x => x.PaymentChannelId)
                .NotEmpty()
                .WithMessage(Resources.BrandPayment.PaymentChannelRequired);

            RuleFor(x => x.TransferLimitTo)
                .GreaterThan(0)
                .WithMessage(Resources.BrandPayment.TransferLimitGreaterZero);

            RuleFor(x => x.DepositedAmountMax)
                .GreaterThan(0)
                .WithMessage(Resources.BrandPayment.DepositedGreaterZero);
        }
    }
}