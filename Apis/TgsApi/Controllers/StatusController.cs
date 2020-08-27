using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.TgsApi.Controllers.Message;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;
using TGS.Infrastructures.Log;
using TGS.Infrastructures.Nats;
using TGS.Infrastructures.Redis;

namespace TGS.Apis.TgsApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ITgsRepository _tgpRepository;
        private readonly IBetRepository _betRepository;
        private readonly IRedisRepository _redisRepository;
        private readonly INatsManager _natsManager;
        private readonly ITgsLog _tgsLog;

        public StatusController(
            ITgsRepository tgpRepository,
            IBetRepository betRepository,
            IRedisRepository redisRepository,
            INatsManager natsManager,
            ITgsLog tgsLog)
        {
            _tgpRepository = tgpRepository;
            _betRepository = betRepository;
            _redisRepository = redisRepository;
            _natsManager = natsManager;
            _tgsLog = tgsLog;
        }

        [HttpGet]
        public async Task<StatusModel> GetStatusAsync()
        {
            var hostName = Dns.GetHostName();
            var hostIp = Array.Find(Dns.GetHostEntry(hostName).AddressList, x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            _tgsLog.LogWarning("test");
            return new StatusModel
            {
                MachineName = Environment.MachineName,
                HostName = hostName,
                HostIp = hostIp.ToString(),
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                OsVersion = $"{Environment.OSVersion}",
                ProcessCount = Environment.ProcessorCount,
                CanConnectTgsDb = await _tgpRepository.CanConnectionAsync(),
                CanConnectBetDb = await _betRepository.CanConnectionAsync(),
                RedisConnectionStatus = _redisRepository.GetConnectionStatus(),
                NatsConnectionStatus = _natsManager.GetConnectStatus()
            };
        }
    }
}
