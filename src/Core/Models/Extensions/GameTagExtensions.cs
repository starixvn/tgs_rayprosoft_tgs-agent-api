using TGS.Core.Models.Enums;
using static TGS.Core.Models.Enums.GameTag;

namespace TGS.Core.Models.Extensions
{
    public static class GameTagExtensions
    {
        public static bool IsNew(this GameTag tag) => tag.Is(New);

        public static bool IsHot(this GameTag tag) => tag.Is(Hot);

        public static bool IsRecommend(this GameTag tag) => tag.Is(Recommend);

        private static bool Is(this GameTag tag, GameTag expected) =>
            (tag & expected) == expected;

        public static GameTag AddTag(this GameTag tags, GameTag newTag)
        {
            if ((tags & newTag) != newTag)
            {
                return tags ^ newTag;
            }

            return tags;
        }

        public static GameTag RemoveTag(this GameTag tags, GameTag oldTag)
        {
            if ((tags & oldTag) == oldTag)
            {
                return tags ^ oldTag;
            }

            return tags;
        }
    }
}