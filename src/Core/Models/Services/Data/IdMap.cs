namespace TGS.Core.Models.Services.Data
{
    public record IdMap<TId> where TId : struct
    {
        public TId Id { get; init; }
        public string ExternalId { get; init; }
    }
}