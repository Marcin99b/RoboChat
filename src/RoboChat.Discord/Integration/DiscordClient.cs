using System;
using System.Threading.Tasks;
using Discord;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using RoboChat.Discord.Controllers;
using RoboChat.Discord.Helpers;
using RoboChat.Discord.Messages;

namespace RoboChat.Discord.Integration
{
    public class DiscordClient
    {
        private readonly ControllersRouter controllersRouter;

        private readonly DiscordSocketClient client;

        private readonly bool IsDebugMode;

        public DiscordClient(ControllersRouter controllersRouter)
        {
            this.controllersRouter = controllersRouter;

            this.client = new DiscordSocketClient(new DiscordSocketConfig
            {
                WebSocketProvider = WS4NetProvider.Instance
            });
            ClientHelper.Initialize(this.client);
        }

        public async Task Run(string token)
        {
            client.Log += Log;
            client.MessageReceived += MessageReceived;
            
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage socketMessage)
        {
            var message = socketMessage.Content;
            
            if (!message.StartsWith("/"))
            {
                return;
            }
            
            await BotMessages.SendResponseWithLoading(socketMessage);

            if (message.StartsWith("/admin"))
            {
                await controllersRouter.Admins(socketMessage);
                return;
            }

            if (!socketMessage.Channel.Name.Contains("bot-talk"))
            {
                return;
            }

            if ((message.StartsWith("/bot") || message.StartsWith("/b"))
                && socketMessage.Channel.Name.Contains("test") != System.Diagnostics.Debugger.IsAttached)
            {
                await controllersRouter.Conversations(socketMessage);
                return;
            }
            
            if (message.StartsWith("/session"))
            {
                await controllersRouter.Sessions(socketMessage);
                return;
            }

            if (message.StartsWith("/room"))
            {
                await controllersRouter.Rooms(socketMessage);
                return;
            }

            await ErrorMessages.SendNotFoundCommand(socketMessage);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
