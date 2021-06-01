namespace TGS.Services.Operator.Services.GameBalance
{
    public record SubmitGameBalanceFundOutCommand(string GameProviderCode, string PlayerCode, int Amount);
}
