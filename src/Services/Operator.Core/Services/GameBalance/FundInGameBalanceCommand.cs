namespace TGS.Services.Operator.Services.GameBalance
{
    public record FundInGameBalanceCommand(string GameProviderCode, string PlayerCode, int Amount);
}
