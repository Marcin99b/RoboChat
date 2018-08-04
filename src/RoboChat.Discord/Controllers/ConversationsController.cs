using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using RoboChat.Discord.Services;

namespace RoboChat.Discord.Controllers
{
    public class ConversationsController : BaseController
    {
        private readonly SessionService sessionService;

        public ConversationsController(SocketMessage socketMessage) : base(socketMessage)
        {
            sessionService = new SessionService();
        }

        public async Task SendMessage(string message)
        {
            await sessionService.SendResponseToUser(socketMessage, message);
        }
    }
}
