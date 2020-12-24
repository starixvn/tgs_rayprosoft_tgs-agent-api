using FluentValidation;
using TGS.Services.BackOffice.Application.Models.BrandGame;

namespace TGS.Services.BackOffice.Validation.BrandGame
{
    public class UpdateBrandGameRequestValidator : AbstractValidator<UpdateBrandGameRequest>
    {
        public UpdateBrandGameRequestValidator()
        {
            RuleFor(x => x.EffectiveBettingPercentage)
                .GreaterThan(-1)
                .WithMessage(Application.Resources.BrandGameProviderSchedule.PercentageLessThan0)
                .LessThan(101)
                .WithMessage(Application.Resources.BrandGameProviderSchedule.PercentageGreaterThan100);
        }
    }
}