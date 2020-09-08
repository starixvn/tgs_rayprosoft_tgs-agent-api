using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.Roles;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validator.Role
{
    public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
    {
        public UpdateRoleRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(Resources.Role.NameRequired);

            RuleFor(x => x.Name)
                .Must(p => p.Length < 50)
                .WithMessage(Resources.Role.NameLengthLimit);
        }
    }
}
