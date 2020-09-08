using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.Roles;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validator.Role
{
    public class CopyRoleRequestValidator : AbstractValidator<CopyRoleRequest>
    {
        public CopyRoleRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(Resources.Role.NameRequired);

            RuleFor(x => x.Name)
                .Must(p => p.Length < 50)
                .WithMessage(Resources.Role.NameLengthLimit);
        }
    }
}
