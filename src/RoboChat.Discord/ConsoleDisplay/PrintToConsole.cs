using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace RoboChat.Discord.ConsoleDisplay
{
    public static class PrintToConsole
    {
        public static PrintMessages(SocketMessage socketMessage, string response)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("");
            Console.WriteLine($"From: {GetFullUsername(socketMessage)}");
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine($"To: {GetFullUsername(socketMessage)}");
            Console.WriteLine(response);
            Console.WriteLine("================");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
