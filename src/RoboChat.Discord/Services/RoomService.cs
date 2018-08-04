using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace RoboChat.Discord.Services
{
    public class RoomService
    {
        public async Task ClearRoom(SocketMessage socketMessage)
        {
            var channel = socketMessage.Channel;
            var messages = await channel.GetMessagesAsync().Flatten();
            await channel.DeleteMessagesAsync(messages);
        }
    }
}
