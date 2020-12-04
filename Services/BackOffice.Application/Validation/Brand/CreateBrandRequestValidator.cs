using FluentValidation;
using TGS.Services.BackOffice.Application.Models.Brand;

namespace TGS.Services.BackOffice.Validation.Brand
{
    public class CreateBrandRequestValidator : AbstractValidator<CreateBrandRequest>
    {
        public CreateBrandRequestValidator()
        {
            Include(new BrandRequestValidator());
        }
    }
}