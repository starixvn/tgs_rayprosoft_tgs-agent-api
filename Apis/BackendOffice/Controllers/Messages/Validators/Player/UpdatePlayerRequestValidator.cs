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
        }
    }
}