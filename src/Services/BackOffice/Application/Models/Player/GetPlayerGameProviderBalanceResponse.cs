namespace TGS.Services.BackOffice.Application.Models.Player;

public class GetPlayerGameProviderBalanceResponse
{
    public decimal? Balance { get; init; }

    public string GameProviderCode { get; init; }

    public string GameProviderName { get; init; }

    public bool IsMainWallet { get; init; }
}
