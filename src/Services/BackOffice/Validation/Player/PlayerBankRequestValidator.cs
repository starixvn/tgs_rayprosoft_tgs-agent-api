using FluentValidation;
using TGS.Services.BackOffice.Application.Models.PlayerBank;

namespace TGS.Services.BackOffice.Validation.Player
{
    public class PlayerBankRequestValidator : AbstractValidator<PlayerBankRequest>
    {
        public PlayerBankRequestValidator()
        {
            RuleFor(p => p.RealName)
                .NotNull()
                .NotEmpty()
                .WithMessage(Application.Resources.Player.BankRealNameRequired);

            RuleFor(p => p.BankName)
                .NotNull()
                .WithMessage(Application.Resources.Player.BankNameRequired);

            RuleFor(p => p.BankAccount)
                .NotNull()
                .WithMessage(Application.Resources.Player.BankAccountRequired);
        }
    }
}