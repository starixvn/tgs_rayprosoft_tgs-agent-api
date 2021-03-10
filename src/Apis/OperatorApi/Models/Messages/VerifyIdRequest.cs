using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public class VerifyIdRequest : IRequest<VerifyIdResponse>
    {
        public string PlayerCode { get; init; }
        public string Id { get; init; }
        public string CurrencyCode { get; init; }
    }
}