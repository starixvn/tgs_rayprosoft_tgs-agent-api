using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;
using TGS.Apis.BackendOffice.Extensions;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validator.Player
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
                .NotNull()
                .WithMessage(Resources.User.PasswordRequired)
                .Equal(p => p.ConfirmedPassword)
                .WithMessage(Resources.User.PasswordNotMatched)
                .Password(); ;
        }
    }
}
