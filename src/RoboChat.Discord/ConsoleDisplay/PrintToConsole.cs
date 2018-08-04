using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using RoboChat.Discord.Helpers;

namespace RoboChat.Discord.ConsoleDisplay
{
    public static class PrintToConsole
    {
        public static void PrintMessages(SocketMessage socketMessage, string message, string response)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("");
            Console.WriteLine($"From: {Userhelper.GetFullUsername(socketMessage)}");
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine($"To: {Userhelper.GetFullUsername(socketMessage)}");
            Console.WriteLine(response);
            Console.WriteLine("================");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
