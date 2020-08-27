using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TGS.Apis.BackendOffice.Controllers.Messages;

namespace TGS.Apis.BackendOffice.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        //private readonly IBackendOfficeRepository _repository;
        //public StatusController(IBackendOfficeRepository repository)
        //{
        //    _repository = repository;
        //}

        private readonly IConfiguration _configuration;
        public StatusController(IConfiguration configuration)
        {
            _configuration = configuration;
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
                //CanConnectBackendOfficeDb = await _repository.CanConnectionAsync(),
                BackendOfficeDatabaseConnectionString = _configuration.GetConnectionString("BackendOffice")
            };
        }
    }
}
