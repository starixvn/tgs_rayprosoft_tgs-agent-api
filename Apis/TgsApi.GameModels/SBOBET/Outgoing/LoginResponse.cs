// ReSharper disable IdentifierTypo

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Outgoing
{
    public record LoginResponse : BaseResponse
    {
        public string Url { get; init; }
    }
}
