using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.PlayerBank
{
    public class CreatePlayerBankRequest : PlayerBankRequest, IRequest<MessageResponse>
    {
    }
}