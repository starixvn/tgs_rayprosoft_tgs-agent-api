using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TGS.Apis.BackendOffice.SignalR;

public class NotifyHub : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
}
