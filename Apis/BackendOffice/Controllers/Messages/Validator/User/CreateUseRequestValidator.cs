using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.Users;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validator.User
{
    public class CreateUseRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUseRequestValidator()
        {
            RuleFor(p => p.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage(Resources.User.NameRequired);

            RuleFor(p => p.Password)
                .NotNull()
                .WithMessage(Resources.User.PasswordRequired)
                .Equal(p => p.ConfirmPassword)
                .WithMessage(Resources.User.PasswordNotMatched);
        }
    }
}
