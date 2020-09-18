using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.Apis.TgsApi.Commons.Filters;
using TGS.Apis.TgsApi.Models;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;
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
        private readonly ITgsLog _tgsLog;
        private readonly ILogger<StatusController> _logger;

        public StatusController(
            ITgsRepository tgpRepository,
            IBetRepository betRepository,
            IRedisRepository redisRepository,
            ITgsLog tgsLog,
            ILogger<StatusController> logger)
        {
            _tgpRepository = tgpRepository;
            _betRepository = betRepository;
            _redisRepository = redisRepository;
            _tgsLog = tgsLog;
            _logger = logger;
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
                Environment = Environment.GetEnvironmentVariable(Consts.EnvironmentVariable.Environment),
                OsVersion = $"{Environment.OSVersion}",
                ProcessCount = Environment.ProcessorCount,
                CanConnectTgsDb = await _tgpRepository.CanConnectionAsync(),
                CanConnectBetDb = await _betRepository.CanConnectionAsync(),
                RedisConnectionStatus = _redisRepository.GetConnectionStatus()
            };
        }

        [HttpGet("/log")]
        public string WriteLogAsync()
        {
            var message = DateTime.Now.ToString();
            _tgsLog.LogWarning(message);
            return message;

        }

        [HttpGet("/nlog")]
        public string WriteNLogAsync()
        {
            var message = DateTime.Now.ToString();
            _logger.LogWarning(message);
            return message;

        }
    }
}
