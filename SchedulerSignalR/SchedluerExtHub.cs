using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerSignalR
{
    public class SchedluerExtHub<THub> where THub : Hub
    {
        public readonly IHubContext<THub> HubContext;

        public SchedluerExtHub(IHubContext<THub> hubContext)
        {
            HubContext = hubContext;
        }

        public async Task SendMessage(string message)
        {
            await HubContext.Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
