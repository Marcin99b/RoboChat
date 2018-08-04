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
using RoboChat.Discord.Services;
using RoboChat.Library;

namespace RoboChat.Discord
{
    public class Program
    {
        private readonly SessionService sessionService;
        private readonly AdminPanel adminPanel;
        private DiscordSocketClient client;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

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
        
        public async Task MainAsync()
        {
            this.client = new DiscordSocketClient(new DiscordSocketConfig
            {
                WebSocketProvider = WS4NetProvider.Instance
            });

            client.Log += Log;
            client.MessageReceived += MessageReceived;

            string token = "NDY5MzU5MTIzMDc5NjI2NzYy.DjGkVQ.5RZG8B0KL0Ml9-tZXeXxKxEN_4I"; // Remember to keep this private!
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage socketMessage)
        {
            var message = socketMessage.ToString();

            // Checking the validity of message
            if (socketMessage.Channel.Name.Contains("admin-panel") && (message.StartsWith("./") || message.StartsWith("/")))
            {
                adminPanel.CheckCommand(socketMessage);
                return;
            }

            if (!message.StartsWith("/"))
            {
                return;
            }

            if (!socketMessage.Channel.Name.Contains("bot-talk"))
            {
                return;
            }

            if (socketMessage.Channel.Name.Contains("test") != isDebugMode)
            {
                return;
            }
            // Here the message is finally checked

            if (message.StartsWith("/bot ") || message.StartsWith("/b "))
            {
                await sessionService.SendResponseToUser(socketMessage);
                return;
            }
            
            if (message.StartsWith("/session -start"))
            {

                await sessionService.CreateNewSession(socketMessage);
                return;
            }

            if (message.StartsWith("/session -delete"))
            {
                await sessionService.DeleteSession(socketMessage);
                return;
            }

            if (message.StartsWith("/session -help"))
            {
                await sessionService.SendResponseWithListOfCommands(socketMessage);
                return;
            }

            if (message.StartsWith("/session -ready"))
            {
                //ready to merge id = 470023263347539979
                var channel = (IMessageChannel) client.GetChannel(470023263347539979);
                await sessionService.ReadyToMerge(socketMessage, channel);
                return;
            }

            if (message.StartsWith("/session -merge"))
            {
                await sessionService.MergeSession(socketMessage);
                return;
            }

            if (message.StartsWith("/session -author"))
            {
                await sessionService.SendResponseWithSessionAuthor(socketMessage);
                return;
            }

            if (message.StartsWith("/room -clear"))
            {
                await sessionService.ClearRoom(socketMessage);
                return;
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}