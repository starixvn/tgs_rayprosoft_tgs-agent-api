using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Messages
{
    public record TgsResponse
    {
        public TgsResponse(TgsApiError errorType = TgsApiError.None, string message = "")
        {
            Error = (int)errorType;
            ErrorMessage = message;
        }
        public int Error { get; init; } = (int)TgsApiError.None;
        public string ErrorMessage { get; init; } = string.Empty;
    }
}
