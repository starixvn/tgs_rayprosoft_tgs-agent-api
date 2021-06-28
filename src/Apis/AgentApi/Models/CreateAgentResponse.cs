namespace TGS.Apis.AgentApi.Models
{
    public class CreateAgentResponse
    {
        public bool IsNew { get; set; }
        public string AgentId { get; set; }
        public string Message { get; set; }
    }
}