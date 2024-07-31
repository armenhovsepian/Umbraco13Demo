using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Umbraco13Demo.Helpers;

namespace Umbraco13Demo.Hubs
{
    public class NotificationHub : Hub
    {
        private const string customerGroup = "Customer";
        private const string adminGroup = "Admin";
        private const string guestGroup = "Guest";

        private static readonly ConcurrentDictionary<string, string> ConnectedClients = new ConcurrentDictionary<string, string>();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (Context.User.Identity.IsAuthenticated)
            {
                var _backofficeUserAccessor = httpContext.RequestServices.GetRequiredService<IBackofficeUserAccessor>();
                if (_backofficeUserAccessor.BackofficeUser != null)
                {
                    Groups.AddToGroupAsync(Context.ConnectionId, adminGroup);
                }
                else
                {
                    Groups.AddToGroupAsync(Context.ConnectionId, customerGroup);
                }

                var userId = Context.User.Identity.Name;
                ConnectedClients.TryAdd(Context.ConnectionId, userId);
            }
            else
            {
                Groups.AddToGroupAsync(Context.ConnectionId, guestGroup);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Groups.RemoveFromGroupAsync(Context.ConnectionId, adminGroup);
            Groups.RemoveFromGroupAsync(Context.ConnectionId, customerGroup);
            Groups.RemoveFromGroupAsync(Context.ConnectionId, guestGroup);

            ConnectedClients.TryRemove(Context.ConnectionId, out var userId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(NotoficationModel model)
        {
            //await Clients.All.SendAsync("ReceiveMessage", message);

            //await Clients.OthersInGroup(guestGroup).SendAsync("ReceiveMessage", $"{model.UserAgent}> {model.Message}");

            await Clients.Group(guestGroup).SendAsync("ReceiveMessage", $"{model.UserAgent}: {model.Message}");
        }
    }

    public class NotoficationModel
    {
        public string UserAgent { get; set; }
        public string Message { get; set; }
    }
}
