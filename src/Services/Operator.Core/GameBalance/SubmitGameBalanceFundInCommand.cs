namespace TGS.Services.Operator.GameBalance
{
    public record SubmitGameBalanceFundInCommand(string GameProviderCode, string PlayerCode, int Amount)
    {
    }
}