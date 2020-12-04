using FluentValidation;
using TGS.Services.BackOffice.Application.Models.Brand;

namespace TGS.Services.BackOffice.Application.Validation.Brand
{
    public class BrandRequestValidator : AbstractValidator<BrandRequest>
    {
        public BrandRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage(Resources.Brand.CodeRequired);
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(Resources.Brand.NameRequired);
            RuleFor(x => x.ApiKey)
                .NotEmpty()
                .WithMessage(Resources.Brand.ApiKeyRequired);
            RuleFor(x => x.SecretKey)
                .NotEmpty()
                .WithMessage(Resources.Brand.SecretKeyRequired);
            RuleFor(x => x.GameProviders)
                .NotEmpty()
                .WithMessage(Resources.Brand.GameProviderRequired);
            RuleForEach(x => x.GameProviders)
                .SetValidator(new BrandGameProviderModelValidator())
                .NotEmpty()
                .WithMessage(Resources.Brand.GameProviderRequired);
            RuleFor(x => x.CurrencyCodes)
                .NotEmpty()
                .WithMessage(Resources.Brand.CurrencyRequired);
            RuleFor(x => x.BrandCultures)
                .NotEmpty()
                .WithMessage(Resources.Brand.CultureRequired);
        }
    }
}