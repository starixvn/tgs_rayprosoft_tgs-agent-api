namespace TGS.Core.Models.Messages
{
    public record TgsResponse
    {
        public int Error { get; init; }
        public string ErrorMessage { get; init; }
    }
}
