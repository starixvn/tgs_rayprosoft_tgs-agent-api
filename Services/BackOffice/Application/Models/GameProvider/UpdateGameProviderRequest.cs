using MediatR;

namespace TGS.Services.BackOffice.Application.Models.GameProvider
{
    public class UpdateGameProviderRequest : GameProviderRequest, IRequest<GameProviderResponse>
    {
    }
}