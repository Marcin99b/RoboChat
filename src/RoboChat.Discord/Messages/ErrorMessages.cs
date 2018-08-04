using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace RoboChat.Discord.Messages
{
    public static class ErrorMessages
    {
        public static async Task SendNotFoundCommand(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync($"**Not found command: {socketMessage.Content}**");
        }
    }
}
