using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.LogApi.Controllers.Messages;
using TGS.Apis.LogApi.DataService;
using TGS.Apis.LogApi.Services;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;

namespace TGS.Apis.LogApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ILogRepository _logRepository;
        private readonly IRedisRepository _redisRepository;
        private readonly IMessageHandler _natsMessageHandler;
        public StatusController(
            ILogRepository logRepository,
            IRedisRepository redisRepository,
            IMessageHandler natsMessageHandler)
        {
            _logRepository = logRepository;
            _redisRepository = redisRepository;
            _natsMessageHandler = natsMessageHandler;
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
                Environment = Environment.GetEnvironmentVariable(Consts.Environment),
                OsVersion = $"{Environment.OSVersion}",
                ProcessCount = Environment.ProcessorCount,
                CanConnectLogDb = await _logRepository.CanConnectionAsync(),
                RedisConnectionStatus = _redisRepository.GetConnectionStatus(),
                last_error = _natsMessageHandler.LastLogInsertError,
                last_log_insert_time = _natsMessageHandler.LastLogInsertTime,
                last_event = _natsMessageHandler.LastBusEvent?.GetType().Name,
                last_received_event_time = _natsMessageHandler.LastReceivedEventTime,
                last_startup_time = _natsMessageHandler.LastStartUpTime,
                log_received_count = _natsMessageHandler.LogReceivedCount,
            };
        }
    }
}
