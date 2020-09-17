using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validator.Player
{
    public class PlayerBankRequestValidator : AbstractValidator<PlayerBankRequest>
    {
        public PlayerBankRequestValidator()
        {
            RuleFor(p => p.Username)
                .NotNull()
                .NotEmpty()
                .WithMessage(Resources.User.NameRequired);

            RuleFor(p => p.BankName)
                .NotNull()
                .WithMessage(Resources.User.PasswordRequired);

            RuleFor(p => p.BankAccount)
                .NotNull()
                .WithMessage(Resources.User.PasswordRequired);
        }
    }
}
