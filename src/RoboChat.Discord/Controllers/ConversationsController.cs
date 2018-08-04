using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace RoboChat.Discord.Controllers
{
    public class ConversationsController : BaseController
    {
        public ConversationsController(SocketMessage socketMessage) : base(socketMessage)
        {
        }

        public async Task SendMessage(string message)
        {

        }
    }
}
