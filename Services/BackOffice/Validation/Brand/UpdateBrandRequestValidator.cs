using System.Linq;
using FluentValidation;
using TGS.Services.BackOffice.Application.Models.Brand;

namespace TGS.Services.BackOffice.Validation.Brand
{
    public class UpdateBrandRequestValidator : AbstractValidator<UpdateBrandRequest>
    {
        public UpdateBrandRequestValidator()
        {
            Include(new BrandRequestValidator());

            RuleFor(x => x.BrandGroups)
                .Must((m, n) => n.Length * m.GameProviders.Length * m.BetLimits.Length ==
                                m.BrandGroups.SelectMany(x => x.GameProviderBetLimits).Count())
                .WithMessage(Application.Resources.Brand.GameProviderBetLimitCountIncorrect);
        }
    }
}