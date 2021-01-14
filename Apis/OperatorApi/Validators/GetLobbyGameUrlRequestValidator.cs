using System.Linq;
using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Validators
{
    public class GetLobbyGameUrlRequestValidator : BaseFluentValidator<GetLobbyGameUrlRequest>
    {
        private readonly GameType[] _gameTypes =
        {
            GameType.Sportsbook, GameType.VirtualSportsLobby, GameType.ArcadeLobby, GameType.CardGameLobby,
            GameType.FishingLobby, GameType.LiveDealerLobby, GameType.LotteryLobby
        };

        public GetLobbyGameUrlRequestValidator()
        {
            RuleFor(x => x.GameProviderCode)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.GameProviderCode)));

            RuleFor(x => x.GameType)
                .IsInEnum()
                .WithName(x => GetJsonPropertyName(x, nameof(x.GameType)))
                .Must(gameType => _gameTypes.Contains(gameType))
                .WithMessage($"Game type must be in ({string.Join(", ", _gameTypes.ToString())})");

            RuleFor(x => x.LanguageCode)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.LanguageCode)));
        }
    }
}