using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using TGS.Apis.TgsApi.Models;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;

namespace TGS.Apis.TgsApi.Commons.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]    
    public class StatusController : ControllerBase
    {
        private readonly ITgsRepository _tgpRepository;
        private readonly IBetRepository _betRepository;
        private readonly IRedisRepository _redisRepository;
        private readonly IFeatureManager _featureManagement;
        private readonly IConfiguration _configuration;

        public StatusController(
            ITgsRepository tgpRepository,
            IBetRepository betRepository,
            IRedisRepository redisRepository,
            IFeatureManager  featureManagement,
            IConfiguration configuration)
        {
            _tgpRepository = tgpRepository;
            _betRepository = betRepository;
            _redisRepository = redisRepository;
            _featureManagement = featureManagement;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<StatusModel> GetStatusAsync()
        {
            var hostName = Dns.GetHostName();
            var hostIp = Array.Find(Dns.GetHostEntry(hostName).AddressList, x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            var gpCode = _configuration.GetValue<string>(Consts.EnvironmentVariable.GameProviderCode);
            var isFeatureEnabled = gpCode.IsNullOrEmpty() ? false : await _featureManagement.IsEnabledAsync(gpCode);

            return new StatusModel
            {
                MachineName = Environment.MachineName,
                HostName = hostName,
                HostIp = hostIp.ToString(),
                Environment = _configuration.GetValue<string>(Consts.EnvironmentVariable.Environment),
                OsVersion = $"{Environment.OSVersion}",
                ProcessCount = Environment.ProcessorCount,
                CanConnectTgsDb = await _tgpRepository.CanConnectionAsync(),
                CanConnectBetDb = await _betRepository.CanConnectionAsync(),
                RedisConnectionStatus = _redisRepository.GetConnectionStatus(),
                GameProviderCode = gpCode,
                FeatureEnabled = isFeatureEnabled
            };
        }
    }
}
