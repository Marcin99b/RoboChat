using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace RoboChat.Discord.Messages
{
    public class BotMessages
    {
        public static async Task SendResponseWithLoading(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync($"**Loading... It may take a few seconds.**");
        }

        public static async Task SendResponseWithInfoAboutOffline(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync("***```If bot \"RoboChat\" is offline, bot is not working!```***");
        }
    }
}
