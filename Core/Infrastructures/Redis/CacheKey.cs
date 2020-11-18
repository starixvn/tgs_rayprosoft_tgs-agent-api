namespace TGS.Core.Infrastructures.Redis
{
    public record CacheKey
    {
        public object CacheType { get; init; }

        public object Key { get; init; }
    }
}