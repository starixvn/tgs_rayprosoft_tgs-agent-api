using MediatR;

namespace TGS.Services.BackOffice.Application.Models.Agent
{
    public record GetAgentDividendSettingRequest(string Username) : IRequest<GetAgentDividendSettingResponse>;
}