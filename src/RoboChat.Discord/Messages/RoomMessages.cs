using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace RoboChat.Discord.Messages
{
    class RoomMessages
    {
        public static async Task CannotClearRoomBcsActiveSession(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync($"```You cannot clear the room, because there is an active session```");
        }
    }
}
