namespace TGS.Core.Models.Services.Games
{
    public record CustomBetValues(
        decimal Turnover,
        decimal WinLoss,
        decimal? BetAmount = null,
        decimal? SettleAmount = null);
}