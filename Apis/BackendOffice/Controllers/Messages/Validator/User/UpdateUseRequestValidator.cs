using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.Users;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validator.User
{
    public class UpdateUseRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUseRequestValidator()
        {
            RuleFor(p => p.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage(Resources.User.NameRequired);
        }
    }
}
