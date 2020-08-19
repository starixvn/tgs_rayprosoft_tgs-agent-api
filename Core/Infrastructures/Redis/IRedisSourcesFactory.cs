namespace TGS.Infrastructures.Redis
{
    public interface IRedisSourcesFactory
    {
        IRedisList<T> GetListSource<T>(string keyPrefix);
        IRedisSortedSet<T> GetSortedSetSource<T>(string keyPrefix);
    }
}
