using TGS.Repositories.ValidateCode;
using TGS.Services.ValidateCode.Configuration;

namespace TGS.Services.ValidateCode
{
    public static class ConfigurationExtensions
    {
        public static ValidateCodeConfiguration UseRedis(this ValidateCodeConfiguration configuration)
        {
            return configuration
                .UseRepository<RedisValidateCodeStore>();
        }
    }
}
