using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Player
{
    public record ClearPlayerBucketsWithDepositRequest(string PlayerCode) : IRequest<MessageResponse>;
}