namespace TGS.Services.BackOffice.Application.Models.Player;

public class GetPlayerGameProviderBalanceCommand
{
    public string PlayerCode { get; set; }

    public GetPlayerGameProviderBalanceCommand(string playerCode) => PlayerCode = playerCode;
}