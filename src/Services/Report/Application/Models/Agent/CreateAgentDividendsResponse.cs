using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class CreateAgentDividendsResponse : MessageResponse
    {
        public CreateAgentDividendModel[] AgentDividends { get; set; }
    }
}