using Microsoft.AspNetCore.SignalR;
using MyChatRoom.Controllers.Dtos;
using MyChatRoom.Services;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace MyChatRoom.Hubs
{
    public class ChatHub : Hub
    {
        public readonly ChatService _chatService;
        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "MyChatRoom");
            await Clients.Caller.SendAsync("UserConnected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "MyChatRoom");
            var user = _chatService.GetUserByConnectionId(Context.ConnectionId);
            _chatService.RemoveUserFromList(user);
            await DisplayOnlineUsers();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task AddUserConnectionId(string name)
        {
            _chatService.AddUserConnectionId(name, Context.ConnectionId);
            var onlineUsers = _chatService.GetOnlineUsers();
            await Clients.Groups("MyChatRoom").SendAsync("OnlineUsers", onlineUsers);
        }

        public async Task CreatePrivateChat(MessageDto message)
        {
            string privateGroupName = GetPrivateGroupName(message.From, message.To);
            await Groups.AddToGroupAsync(Context.ConnectionId, privateGroupName);
            var toConnectionId = _chatService.GetConnectionIdByUser(message.To);
            await Groups.AddToGroupAsync(toConnectionId, privateGroupName);
            // opening private chatbox for the other end user
            await Clients.Client(toConnectionId).SendAsync("OpenPrivateChat", message);
        }

        public async Task ReceivePrivateMessage(MessageDto message)
        {
            string privateGroupName = GetPrivateGroupName(message.From, message.To);
            await Clients.Group(privateGroupName).SendAsync("NewPrivateMessage", message);
        }

        public async Task RemovePrivateChat(string from, string to)
        {
            string privateGroupName = GetPrivateGroupName(from, to);
            await Clients.Group(privateGroupName).SendAsync("ClosePrivateChat");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, privateGroupName);
            var toConnectionId = _chatService.GetConnectionIdByUser(to);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, privateGroupName);
        }


        private async Task DisplayOnlineUsers()
        {
            var onlinUsers = _chatService.GetOnlineUsers();
            await Clients.Groups("MyChatRoom").SendAsync("OnlineUsers", onlinUsers);
        }

        public async Task ReceiveMessage(MessageDto message)
        {
            await Clients.Group("MyChatRoom").SendAsync("NewMessage", message);
        }

        private string GetPrivateGroupName(string from, string to)
        {
            // from:john , to :devid "devid-jhon"
            var stringCompare = string.CompareOrdinal(from, to) < 0;
            return stringCompare ? from : to;
        }
    }
}
