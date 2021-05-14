namespace TGS.Services.Operator.GameBalance
{
    public record SubmitGameBalanceFundOutCommand(string GameProviderCode, string PlayerCode, int Amount);
}