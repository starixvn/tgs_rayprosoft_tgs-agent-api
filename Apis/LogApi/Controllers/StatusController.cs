using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.LogApi.Controllers.Messages;
using TGS.Apis.LogApi.DataService;

namespace TGS.Apis.LogApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly LogDbContext _context;

        public StatusController(LogDbContext context)
        {
            _context = context;
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
                HostIP = hostIp.ToString(),
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                OSVersion = $"{Environment.OSVersion}",
                ProcessCount = Environment.ProcessorCount,
                CanConnectDB = await _context.Database.CanConnectAsync()
            };
        }
    }
}
