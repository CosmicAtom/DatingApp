using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SingalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;

        private readonly ILogger<PresenceHub> _logger;

        public PresenceHub(PresenceTracker presenceTracker, ILogger<PresenceHub> logger)
        {
            _presenceTracker = presenceTracker;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            //_logger.LogInformation("Context.ConnectionId" + Context.ConnectionId);

            var online = await _presenceTracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            if (online)
            {
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
            }

            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var offline = await _presenceTracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            if (offline)
            {
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
