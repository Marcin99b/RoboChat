using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using RoboChat.Discord.Messages;
using RoboChat.Discord.Services;

namespace RoboChat.Discord.Controllers
{
    public class RoomsController : BaseController
    {
        private readonly RoomService roomService;

        public RoomsController(SocketMessage socketMessage) : base(socketMessage)
        {
            roomService = new RoomService();
        }

        [Command("-clear")]
        public async Task ClearRoom()
        {
            if (SessionService.GetThisRoomChatSession(socketMessage) != null)
            {
                await RoomMessages.CannotClearRoomBcsActiveSession(socketMessage);
                return;
            }
            await roomService.ClearRoom(socketMessage);
            await SessionMessages.SendResponseWithListOfCommands(socketMessage);
            await BotMessages.SendResponseWithInfoAboutOffline(socketMessage);
        }
    }
}
