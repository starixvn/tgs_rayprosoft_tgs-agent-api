using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.LogApi.Controllers.Messages;
using TGS.Apis.LogApi.DataService;
using TGS.Infrastructures.Nats;
using TGS.Infrastructures.Redis;

namespace TGS.Apis.LogApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ILogRepository _logRepository;
        private readonly IRedisRepository _redisRepository;
        private readonly INatsManager _natsManager;
        public StatusController(
            ILogRepository logRepository,
            IRedisRepository redisRepository,
            INatsManager natsManager)
        {
            _logRepository = logRepository;
            _redisRepository = redisRepository;
            _natsManager = natsManager;
        }

        [HttpGet]
        public async Task<StatusModel> GetStatusAsync()
        {
            var hostName = Dns.GetHostName();
            var hostIp = Array.Find(Dns.GetHostEntry(hostName).AddressList, x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            return new StatusModel
            {
                MachineName = Environment.MachineName,
                HostName = hostName,
                HostIp = hostIp.ToString(),
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                OsVersion = $"{Environment.OSVersion}",
                ProcessCount = Environment.ProcessorCount,
                CanConnectLogDb = await _logRepository.CanConnectionAsync(),
                RedisConnectionStatus = _redisRepository.GetConnectionStatus(),
                NatsConnectionStatus = _natsManager.GetConnectStatus()
            };
        }
    }
}
