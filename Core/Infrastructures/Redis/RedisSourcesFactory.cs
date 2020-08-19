namespace TGS.Infrastructures.Redis
{
    public sealed class RedisSourcesFactory : IRedisSourcesFactory
    {
        private readonly IUnityContainer _container;

        public RedisSourcesFactory(IUnityContainer container)
        {
            _container = container;
        }
        
        IRedisList<T> IRedisSourcesFactory.GetListSource<T>(string keyPrefix)
        {
            var source = _container.Resolve<IRedisList<T>>();
            source.SetKeyPrefix($"{source.GetType().Name}:{keyPrefix}");
            return source;
        }

        IRedisSortedSet<T> IRedisSourcesFactory.GetSortedSetSource<T>(string keyPrefix)
        {
            var source = _container.Resolve<IRedisSortedSet<T>>();
            source.SetKeyPrefix($"{source.GetType().Name}:{keyPrefix}");
            return source;
        }
    }
}
