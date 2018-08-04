using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace RoboChat.Discord.Controllers
{
    public class AdminsController : BaseController
    {
        [Command("test")]
        public void Test()
        {

        }

        public AdminsController(SocketMessage socketMessage) : base(socketMessage)
        {
        }
    }
}
