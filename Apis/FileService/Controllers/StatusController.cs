using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.FileService.DataService;
using TGS.Apis.FileService.DataService.Model;
using TGS.Core.Models;

namespace TGS.Apis.FileService.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IRepository<File> _file;
        private readonly IDatabaseSettings _settings;

        public StatusController(IDatabaseSettings settings, IRepository<File> file)
        {
            _settings = settings;
            _file = file;
        }

        [HttpGet]
        [Description("Get status")]
        public async Task<dynamic> GetStatusAsync()
        {
            var hostName = Dns.GetHostName();
            var hostIp = Array.Find(Dns.GetHostEntry(hostName).AddressList,
                x => x.AddressFamily == AddressFamily.InterNetwork);
            return new
            {
                Environment.MachineName,
                HostName = hostName,
                HostIp = hostIp.ToString(),
                Environment = Environment.GetEnvironmentVariable(Consts.EnvironmentVariable.Environment),
                OsVersion = $"{Environment.OSVersion}",
                ProcessCount = Environment.ProcessorCount,
                CanConnectDb = await _file.CanConnection(),
                FileConnectionString = _settings.ConnectionString
            };
        }
    }
}