namespace TGS.Services.Operator.Services.GameBalance
{
    public record FundOutGameBalanceCommand(string GameProviderCode, string PlayerCode, int Amount);
}
