using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Player
{
    public class ClearPlayerBucketsRequest : BaseRequest, IRequest<MessageResponse>
    {
        public string PlayerCode { get; set; }

        public ClearPlayerBucketsRequest(string playerCode)
        {
            PlayerCode = playerCode;
        }
    }
}