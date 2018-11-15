using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace RoboChat.Discord.Helpers
{
    public static class Userhelper
    {
        public static string GetFullUsername(SocketMessage socketMessage)
            => $"{socketMessage.Author.Username}#{socketMessage.Author.Discriminator}";

        public static bool IsAdmin(SocketMessage message)
        {
            return true;
        }
        // => ((SocketGuildUser)message.Author).Roles.Any(role => role.Name == "administracja");
    }
}
