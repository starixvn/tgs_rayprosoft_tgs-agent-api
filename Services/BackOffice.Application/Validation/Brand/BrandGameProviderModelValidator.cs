using FluentValidation;
using TGS.Services.BackOffice.Application.Models.Brand;

namespace TGS.Services.BackOffice.Application.Validation.Brand
{
    internal class BrandGameProviderModelValidator : AbstractValidator<BrandGameProviderModel>
    {
        public BrandGameProviderModelValidator()
        {
            RuleFor(x => x.Configuration.ClientId)
                .NotEmpty()
                .WithMessage(Resources.Brand.ClientIdRequired);

            RuleFor(x => x.Configuration.SecretKey)
                .NotEmpty()
                .WithMessage(Resources.Brand.SecretKeyRequired);

            RuleFor(x => x.Configuration.Status)
                .IsInEnum()
                .WithMessage(Resources.Brand.StatusRequired);

            RuleFor(x => x.Configuration.DesktopEndpoint)
                .NotEmpty()
                .WithMessage(Resources.Brand.DesktopEndpointRequired);

            RuleFor(x => x.Configuration.MobileEndpoint)
                .NotEmpty()
                .WithMessage(Resources.Brand.MobileEndpointRequired);

            RuleFor(x => x.Configuration.ConfigurationData)
                .NotEmpty()
                .WithMessage(Resources.Brand.ConfigurationDataRequired);

            RuleFor(x => x.Configuration.IsDefault)
                .NotEmpty()
                .WithMessage(Resources.Brand.IsDefaultRequired);
        }
    }
}