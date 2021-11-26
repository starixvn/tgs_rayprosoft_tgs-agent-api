using MediatR;

namespace TGS.Services.BackOffice.Application.Models.Player
{
    public class GetPlayerGameProviderBalanceRequest : IRequest<GetPlayerGameProviderBalanceResponse[]>
    {
        public string PlayerCode { get; set; }

        public GetPlayerGameProviderBalanceRequest(string playerCode)
        {
            PlayerCode = playerCode;
        }
    }
}
