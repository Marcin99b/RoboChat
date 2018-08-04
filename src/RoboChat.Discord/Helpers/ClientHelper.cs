using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace RoboChat.Discord.Helpers
{
    public static class ClientHelper
    {
        private static DiscordSocketClient client;

        public static void Initialize(DiscordSocketClient client)
        {
            ClientHelper.client = client;
        }

        public static IMessageChannel GetReadyToMergeChannel()
        {
            return (IMessageChannel)client.GetChannel(470023263347539979);
        }
    }
}
