using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Games
{
    public record PlayerValidationData
    {
        public string PlayerCode { get; init; }
        public string GameProviderCode { get; init; }
        public PlatformType? PlatformType {  get; init; }
    }
}