using FluentValidation;
using TGS.Services.BackOffice.Application.Models.Brand;

namespace TGS.Services.BackOffice.Validation.Brand
{
    public class BrandRequestValidator : AbstractValidator<BrandRequest>
    {
        public BrandRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.CodeRequired);
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.NameRequired);
            RuleFor(x => x.ApiKey)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.ApiKeyRequired);
            RuleFor(x => x.SecretKey)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.SecretKeyRequired);
            RuleFor(x => x.GameProviders)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.GameProviderRequired);
            RuleForEach(x => x.GameProviders)
                .SetValidator(new BrandGameProviderModelValidator())
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.GameProviderRequired);
            RuleFor(x => x.CurrencyCodes)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.CurrencyRequired);
            RuleFor(x => x.BrandCultures)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.CultureRequired);
        }
    }
}