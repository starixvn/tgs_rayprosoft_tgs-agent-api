using System;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Jili.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    internal class JiliActionModelMapper : IJiliActionModelMapper
    {
        private readonly IJiliPlayerService _playerService;

        public JiliActionModelMapper(
            IJiliPlayerService playerService)
        {
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public JiliAuthorizePlayerResponse AuthorizePlayerResponse(string playerCode, PlayerValidationResult result)
            => new JiliAuthorizePlayerResponse
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty,
                Balance = result.Balance,
                JiliPlayerId = _playerService.ToJiliPlayerId(result.BrandCode, playerCode),
                CurrencyCode = result.GameProviderCurrencyCode
            };

        public PlayerValidationData ValidateRequest(string playerCode, PlatformType platformType)
            => new PlayerValidationData
            {
                PlayerCode = playerCode,
                PlatformType = platformType,
                GameProviderCode = Consts.GameProvider.Jili
            };
    }
}
