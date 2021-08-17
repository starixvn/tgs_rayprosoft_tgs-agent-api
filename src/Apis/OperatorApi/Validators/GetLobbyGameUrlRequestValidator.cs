using System.Linq;
using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Validators
{
    public class GetLobbyGameUrlRequestValidator : BaseFluentValidator<GetLobbyGameUrlRequest>
    {
        private readonly GameType[] _allowedGameTypes =
        {
            GameType.Sportsbook, GameType.VirtualSportsLobby, GameType.ESports, 
            GameType.LiveDealerLobby, GameType.CardGameLobby, GameType.FishingLobby, 
            GameType.ArcadeLobby, GameType.LotteryLobby
        };

        public GetLobbyGameUrlRequestValidator()
        {
            RuleFor(x => x.SubGameProviderCode)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.SubGameProviderCode)));

            RuleFor(x => x.GameType)
                .IsInEnum()
                .Must(gameType => _allowedGameTypes.Contains(gameType))
                .WithMessage(x =>
                    $"{GetBindPropertyName(x, nameof(x.GameType))} must be in ({_allowedGameTypes.Select(t => (int) t).JoinAsStringByComma()})");

            RuleFor(x => x.LanguageCode)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.LanguageCode)));
        }
    }
}