using FluentValidation;
using TGS.Clients.Report.Client.Models.Player;

namespace ReportApi.Validations.Player
{
    public class GetPlayerAccountChangesRequestValidator : AbstractValidator<GetPlayerAccountChangesRequest>
    {
        public GetPlayerAccountChangesRequestValidator()
        {
            RuleFor(p => p.PlayerCode)
                .NotEmpty()
                .WithMessage(Resources.Player.PlayerCodeRequired);
        }
    }
}