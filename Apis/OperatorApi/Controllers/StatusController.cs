using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.DataServices;
using EnvironmentVariable = TGS.Core.Models.Consts.EnvironmentVariable;

namespace TGS.Apis.OperatorApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ITgsRepository _repository;
        public StatusController(ITgsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet, Description("Get status")]
        public async Task<StatusModel> GetStatusAsync()
        {
            var hostName = Dns.GetHostName();
            var hostIp = Array.Find(Dns.GetHostEntry(hostName).AddressList, x => x.AddressFamily == AddressFamily.InterNetwork);
            return new StatusModel
            {
                MachineName = Environment.MachineName,
                HostName = hostName,
                HostIp = hostIp.ToString(),
                Environment = Environment.GetEnvironmentVariable(EnvironmentVariable.Environment),
                OsVersion = $"{Environment.OSVersion}",
                ProcessCount = Environment.ProcessorCount,
                CanConnectTgsDb = await _repository.CanConnectionAsync(),
                TgsDatabaseConnectionString = _repository.GetDbConnection().ConnectionString
            };
        }
    }
}
