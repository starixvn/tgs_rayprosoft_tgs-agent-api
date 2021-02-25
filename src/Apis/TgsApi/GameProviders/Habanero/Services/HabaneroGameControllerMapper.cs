using System;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Services
{
    public interface IHabaneroGameControllerMapper
    {
        PlayerValidationData Request(string playerCode, PlatformType platformType);
        AuthorizePlayerResponse Response(string playerCode, PlayerValidationResult r);
        AuthorizePlayerResponse Response(string playerCode, GetBalanceResult r);
    }

    public sealed class HabaneroGameControllerMapper : IHabaneroGameControllerMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Habanero;

        PlayerValidationData IHabaneroGameControllerMapper.Request(string playerCode, PlatformType platformType) =>
            new()
            {
                GameProviderCode = GameProviderCode,
                PlayerCode = playerCode,
                PlatformType = platformType
            };

        AuthorizePlayerResponse IHabaneroGameControllerMapper.Response(string playerCode, PlayerValidationResult r) =>
            new()
            {
                Result = new()
                {
                    Status = new(),
                    PlayerCode = playerCode,
                    PlayerUsername = r.Username,
                    Balance = r.Balance,
                    CurrencyCode = r.GameProviderCurrency
                }
            };

        AuthorizePlayerResponse IHabaneroGameControllerMapper.Response(string playerCode, GetBalanceResult r) =>
            new()
            {
                Result = new()
                {
                    Status = new(),
                    PlayerCode = playerCode,
                    PlayerUsername = r.Username,
                    Balance = r.Balance,
                    CurrencyCode = r.GameProviderCurrency
                }
            };
    }
}