using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using RoboChat.Discord.Controllers;
using RoboChat.Discord.Helpers;
using RoboChat.Discord.Integration;
using RoboChat.Discord.Services;
using RoboChat.Library;

namespace RoboChat.Discord
{
    public class Program
    {
        private readonly SessionService sessionService;
        private readonly AdminPanel adminPanel;

        public static void Main(string[] args)
        {
            new DiscordClient(new ControllersRouter())
                .Run("NTEyMzAxMTY2NzYwMjk2NDQ4.Ds3dnA.QyufpOCi_-PEJmcP1rR0MzB1jUk")
                .GetAwaiter().GetResult();
        } 

        private readonly Boolean isDebugMode;

        public Program()
        {
            #if DEBUG
                isDebugMode = true;
                Console.WriteLine("Program is started in debug mode\n");
            #else
                isDebugMode = false;
                Console.WriteLine("Program is started in release mode\n");
            #endif
            sessionService = new SessionService();
            adminPanel = new AdminPanel(sessionService.returnChatSessionList(), isDebugMode);
        }

        
    }
}