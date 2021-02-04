using MediatR;

namespace TGS.Services.BackOffice.Application.Models.GameProvider
{
    public class CreateGameProviderRequest : GameProviderRequest, IRequest<GameProviderResponse>
    {
    }
}