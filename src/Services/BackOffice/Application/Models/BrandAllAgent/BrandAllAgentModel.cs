namespace TGS.Services.BackOffice.Application.Models.BrandAllAgent
{
    public record BrandAllAgentModel
    {
        public bool Switch { get; set; }

        public BrandAllAgentGameTypeModel[] BrandAllAgentGameTypes { get; set; }
    }
}