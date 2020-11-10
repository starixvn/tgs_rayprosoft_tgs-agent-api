using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validators.Player
{
    public class UpdatePlayerRequestValidator : AbstractValidator<UpdatePlayerRequest>
    {
        public UpdatePlayerRequestValidator()
        {
            RuleFor(p => p.RealName)
                .NotNull()
                .NotEmpty()
                .WithMessage(Resources.User.NameRequired);

            RuleFor(p => p.Password)
                .Equal(p => p.ConfirmedPassword).When(p => p.Password != null)
                .WithMessage(Resources.User.PasswordNotMatched)
                .NotEmpty().When(p => p.Password != null).WithMessage(Resources.User.PasswordRequired)
                .MinimumLength(8).When(p => p.Password != null).WithMessage(Resources.User.PasswordLengthAtLeast)
                .Matches("[0-9]").When(p => p.Password != null).WithMessage(Resources.User.PasswordNeedNumber)
                .Matches("[^a-zA-Z0-9]").When(p => p.Password != null)
                .WithMessage(Resources.User.PasswordNeedSpecialCharacter);
        }
    }
}