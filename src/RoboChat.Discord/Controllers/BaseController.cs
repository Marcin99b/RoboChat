using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace RoboChat.Discord.Controllers
{
    public abstract class BaseController
    {
        protected readonly SocketMessage socketMessage;

        protected BaseController(SocketMessage socketMessage)
        {
            this.socketMessage = socketMessage;
        }
    }
}
