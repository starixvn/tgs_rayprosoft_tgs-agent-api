## Since we all use redis for cache, for easier way to clear the caches by Backoffice,
## please do follow the instructions to set redis cache type and key:

# for the topest table type, like Brand / GameProvider / Player etc..
1. set a redis cache type and  use recode id as key (brandId / gameproviderId ...)
2. determine the value to store the data into hash or by json string

e.g.
a. CachedGameProvider:
RedisCacheType: GameProvider
CacheKey: gameProviderId
Data: store each value by hash 

b. GameProviderSupportedLanguage:
RedisCacheType: GameProviderLanguages
CacheKey: gameProvider
Data: store dictionary as json string


# for the sub table type under topest table type, like BrandGameProviderConfiguration / GameProviderConfiguration
1. set a redis cache type and  use recode id as key (brandId / gameproviderId ...)
2. store data into hash with rest query keys, key format should be concated by colon (e.g. A:B:C) for multiple query keys.
3. if stored data is complex type, serialize the object to json string before storing into hash.

e.g.
a. CachedGameProviderConfiguration:
RedisCacheType: GameProviderConfiguration, 
CacheKey: gameProviderId, 
HashField: gameProviderConfigurationId, 
Data: cached game provider object as json string

b. CachedBrandGameProviderConfiguration:
RedisCacheType: BrandGameProviderConfiguration, 
CacheKey: brandId, 
HashField: gameProviderId, 
Data: cached brand game provider object as json string