﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using RoboChat.Discord.Helpers;
using RoboChat.Discord.Messages;
using RoboChat.Discord.Services;

namespace RoboChat.Discord.Controllers
{
    public class SessionsController : BaseController
    {
        private readonly SessionService sessionService;
        private readonly RoomService roomService;

        public SessionsController(SocketMessage socketMessage) : base(socketMessage)
        {
            this.sessionService = new SessionService();
            this.roomService = new RoomService();
        }

        [Command("-start")]
        public async Task StartSession()
        { 
            await BotMessages.SendResponseWithLoading(socketMessage);
            await roomService.ClearRoom(socketMessage);
            await sessionService.CreateNewSession(socketMessage);
            await SessionMessages.SendResponseWithListOfCommands(socketMessage);
            
        }
        
        [Command("-ready")]
        public async Task ReadySession()
        {
            var channel = ClientHelper.GetReadyToMergeChannel();
            await sessionService.ReadyToMerge(socketMessage, channel);
        }

        [Command("-merge")]
        public async Task MergeSession()
        {
            await sessionService.MergeSession(socketMessage);
        }
        
        [Command("-delete")]
        public async Task DeleteSession()
        {
            await sessionService.DeleteSession(socketMessage);
            await roomService.ClearRoom(socketMessage);
        }

        [Command("-help")]
        public async Task HelpSession()
        {
            await SessionMessages.SendResponseWithListOfCommands(socketMessage);
        }

        [Command("-author")]
        public async Task AuthorSession()
        {
            await sessionService.SendResponseWithSessionAuthor(socketMessage);
        }
    }
}
