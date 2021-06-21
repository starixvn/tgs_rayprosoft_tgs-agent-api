namespace TGS.Services.Operator.Services.Auth
{
    public record GetAuthQuery(string ApiKey, string ClaimNameCurrency, string Currency, string Site);
}