using FluentValidation;
using TGS.Services.BackOffice.Application.Models.Brand;

namespace TGS.Services.BackOffice.Validation.Brand
{
    public class BrandGameProviderModelValidator : AbstractValidator<BrandGameProviderModel>
    {
        public BrandGameProviderModelValidator()
        {
            RuleFor(x => x.Configuration.ClientId)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.ClientIdRequired);

            RuleFor(x => x.Configuration.SecretKey)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.SecretKeyRequired);

            RuleFor(x => x.Configuration.Status)
                .IsInEnum()
                .WithMessage(Application.Resources.Brand.StatusRequired);

            RuleFor(x => x.Configuration.DesktopEndpoint)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.DesktopEndpointRequired);

            RuleFor(x => x.Configuration.MobileEndpoint)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.MobileEndpointRequired);

            RuleFor(x => x.Configuration.ConfigurationData)
                .NotEmpty()
                .WithMessage(Application.Resources.Brand.ConfigurationDataRequired);
        }
    }
}