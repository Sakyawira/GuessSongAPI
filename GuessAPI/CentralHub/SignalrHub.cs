using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace GuessAPI.CentralHub
{
    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }

    public class SignalRHub : Hub
    {
        // Inform all clients that another client has joined
        public async Task BroadcastMessage()
        {
            await Clients.All.SendAsync("Join");
        }

        // Inform all clients that a video has been added
        public async Task AddVideo()
        {
            await Clients.All.SendAsync("VideoAdded");
        }

        // Inform all clients that a video has been deleted
        public async Task DeleteVideo()
        {
            await Clients.All.SendAsync("VideoDeleted");
        }

        // Execute at the start of connection
        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            int usersCount = UserHandler.ConnectedIds.Count();
            Clients.All.SendAsync("CountUsers", usersCount);
            return base.OnConnectedAsync();
        }

        // Execute at the end of connection
        public override Task OnDisconnectedAsync(Exception ex)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            int usersCount = UserHandler.ConnectedIds.Count();
            Clients.All.SendAsync("CountUsers", usersCount);
            return base.OnDisconnectedAsync(ex);
        }
    }
}