using FluentValidation;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validators.Player
{
    public class PlayerBankRequestValidator : AbstractValidator<PlayerBankRequest>
    {
        public PlayerBankRequestValidator()
        {
            RuleFor(p => p.RealName)
                .NotNull()
                .NotEmpty()
                .WithMessage(Resources.Player.BankRealNameRequired);

            RuleFor(p => p.BankName)
                .NotNull()
                .WithMessage(Resources.Player.BankNameRequired);

            RuleFor(p => p.BankAccount)
                .NotNull()
                .WithMessage(Resources.Player.BankAccountRequired);
        }
    }
}