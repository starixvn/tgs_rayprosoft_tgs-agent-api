using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.DataServices;
using TGS.TgsApi.Controllers.Message;

namespace TGS.TgsApi.Controllers
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
                CanConnectDb = await _repository.CanConnectionAsync()
            };
        }
    }
}
