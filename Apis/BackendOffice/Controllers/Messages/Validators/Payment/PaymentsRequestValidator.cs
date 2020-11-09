using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validators.Payment
{
    public class PaymentsRequestValidator : AbstractValidator<PaymentsRequest>
    {
        public PaymentsRequestValidator()
        {
            Include(new PageableRequestValidator());
        }
    }
}