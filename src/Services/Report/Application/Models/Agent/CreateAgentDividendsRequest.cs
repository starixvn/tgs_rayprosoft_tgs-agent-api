using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class CreateAgentDividendsRequest : IRequest<MessageResponse>
    {
        public string BrandCode { get; set; }
    }
}