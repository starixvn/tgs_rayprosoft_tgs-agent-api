using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public record GameInfo
    {
        public int Id { get; init; }

        public string Code { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public string GameProviderCode { get; init; }

        public bool IsActive { get; init; }

        public GameType Type { get; init; }

        public TagInfo Tag { get; init; }

        public int DisplayOrder { get; init; }
    }
}