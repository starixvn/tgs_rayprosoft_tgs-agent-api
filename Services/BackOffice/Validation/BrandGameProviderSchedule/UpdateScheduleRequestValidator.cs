using FluentValidation;
using TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule;

namespace TGS.Services.BackOffice.Validation.BrandGameProviderSchedule
{
    public class UpdateScheduleRequestValidator : AbstractValidator<UpdateScheduleRequest>
    {
        public UpdateScheduleRequestValidator()
        {
            Include(new ScheduleRequestValidator());
        }
    }
}