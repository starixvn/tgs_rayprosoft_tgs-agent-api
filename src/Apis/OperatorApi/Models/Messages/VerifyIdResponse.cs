using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record VerifyIdResponse : TgsResponse
    {
        public bool Validated { get; init; }
    }
}