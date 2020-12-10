using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Application.Services
{
    public static class GameTagExtensions
    {
        private const GameTag List = GameTag.Hot | GameTag.New | GameTag.Recommend;

        public static bool HasFlag(this GameTag tag)
        {
            return (List & tag) == tag;
        }

        public static GameTag AddFlag(this GameTag tag, GameTag newTag)
        {
            return tag | newTag;
        }

        public static GameTag RemoveFlag(this GameTag tag, GameTag oldTag)
        {
            return tag ^ oldTag;
        }
    }
}