using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using RoboChat.Library;

namespace RoboChat.Discord.Helpers
{
    public static class SessionHelper
    {
        public static bool IsSessionInThisRoom(SocketMessage socketMessage, IEnumerable<ChatSession> sessions)
        {
            return sessions.Any(x => x.RoomName == socketMessage.Channel.Name);
        }
    }
}
