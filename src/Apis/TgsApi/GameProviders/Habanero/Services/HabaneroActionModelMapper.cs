using System;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Habanero.Enums;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Services
{
    public interface IHabaneroActionModelMapper
    {
        PlayerValidationData Request(string playerCode, PlatformType platformType);

        BetData Request(GameProviderConfigurationData data, FundTransferRequest request);

        AuthorizePlayerResponse Response(string playerCode, PlayerValidationResult r);

        AuthorizePlayerResponse Response(string playerCode, GetBalanceResult r);

        AuthorizePlayerResponse Response(GameApiError error, string message);
    }

    public sealed class HabaneroActionModelMapper : IHabaneroActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Habanero;

        PlayerValidationData IHabaneroActionModelMapper.Request(string playerCode, PlatformType platformType) =>
            new()
            {
                GameProviderCode = GameProviderCode,
                PlayerCode = playerCode,
                PlatformType = platformType
            };

        AuthorizePlayerResponse IHabaneroActionModelMapper.Response(string playerCode, PlayerValidationResult r) =>
            new()
            {
                Result = new()
                {
                    Status = new() {HasAuthenticationError = false},
                    PlayerCode = playerCode,
                    PlayerUsername = r.Username,
                    Balance = r.Balance,
                    CurrencyCode = r.GameProviderCurrency
                }
            };

        BetData IHabaneroActionModelMapper.Request(GameProviderConfigurationData data, FundTransferRequest request) =>
            throw new NotImplementedException();

        AuthorizePlayerResponse IHabaneroActionModelMapper.Response(string playerCode, GetBalanceResult r) =>
            new()
            {
                Result = new()
                {
                    Status = new() {HasAuthenticationError = false},
                    PlayerCode = playerCode,
                    PlayerUsername = r.Username,
                    Balance = r.Balance,
                    CurrencyCode = r.GameProviderCurrency
                }
            };

        AuthorizePlayerResponse IHabaneroActionModelMapper.Response(GameApiError error, string message) =>
            new()
            {
                Result = new()
                {
                    Status = new()
                    {
                        Success = false,
                        Message = $"code: {(int)error}, message: {message}"
                    }
                }
            };
    }
}