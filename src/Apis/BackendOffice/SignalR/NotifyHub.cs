using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TGS.Core.Models;

namespace TGS.Apis.BackendOffice.SignalR;

public class NotifyHub : Hub
{
    private readonly ILogger<NotifyHub> _logger;

    public NotifyHub(ILogger<NotifyHub> logger)
    {
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("{0}", Context.User.FindFirstValue(Consts.BackendOffice.UserInfo.UserIdKey));

        return base.OnConnectedAsync();
    }
}
