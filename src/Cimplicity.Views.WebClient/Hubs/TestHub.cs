using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cimplicity.Views.WebClient.Hubs
{
    public class TestHub : Hub
    {
        private static readonly Stack<string> _connectionIds = new Stack<string>();
        private static readonly IDictionary<string, string> _clientsInfo = new Dictionary<string, string>();
        public override Task OnConnectedAsync()
        {
            if (!_connectionIds.Contains(Context.ConnectionId))
                _connectionIds.Push(Context.ConnectionId);

            return base.OnConnectedAsync();
        }
        public async Task SendToAll(string name, string message)
        {
            await Clients.All.SendAsync("sendToAll", name, message);
        }
        public async Task Connect(string nick)
        {
            if (!_clientsInfo.ContainsKey(nick))
                _clientsInfo.Add(nick, Context.ConnectionId);
            await this.GetConnectedUsers();
        }
        public async Task SendToClient(string sendTo, string message)
        {
            await Clients.Client(_clientsInfo[sendTo]).SendAsync("sendToClient", _clientsInfo.SingleOrDefault(x => x.Value == Context.ConnectionId).Key, message);
        }
        public async Task GetConnectedUsers()
        {
            var c = (_clientsInfo
                .Select(x => new ClientInfo() { ConnectionId = x.Value, Nickname = x.Key })
                .ToList());
            await Clients.All.SendAsync("getConnectedUsers", c);
        }
    }
    public class ClientInfo
    {
        public string Nickname { get; set; }
        public string ConnectionId { get; set; }
    }
}
