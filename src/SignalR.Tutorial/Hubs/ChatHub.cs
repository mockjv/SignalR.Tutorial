using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Tutorial.Hubs
{
    [Authorize]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ChatHub : Hub
    {
        private readonly IDictionary<string, string> _activeSessions = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            _activeSessions[Context.ConnectionId] = Context.User.Identity.Name;
            await Clients.All.SendAsync("SendAction", Context.User.Identity.Name, "joined",
                _activeSessions.Values.Distinct());
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _activeSessions.Remove(Context.ConnectionId);
            return Clients.All.SendAsync("SendAction", Context.User.Identity.Name, "left",
                _activeSessions.Values.Distinct());
        }

        public async Task Send(string message)
        {
            await Clients.All.SendAsync("SendMessage", Context.User.Identity.Name, message);
        }
    }
}