﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WebSocket;
using RoboChat.Discord.Messages;

namespace RoboChat.Discord.Controllers
{
    public class ControllersRouter
    {

        public async Task Conversations(SocketMessage socketMessage)
        {
            var message = socketMessage.Content.TrimStart("/bot ".ToCharArray());

            var controller = new ConversationsController(socketMessage);
            await controller.SendMessage(message);
        }

        public async Task Admins(SocketMessage socketMessage)
        {
            var message = socketMessage.Content.TrimStart("/admin ".ToCharArray());

            var controller = new AdminsController(socketMessage);
            await this.RunMethodInController(controller, socketMessage, message);
        }

        public async Task Sessions(SocketMessage socketMessage)
        {
            var message = socketMessage.Content.TrimStart("/session ".ToCharArray());

            var controller = new SessionsController(socketMessage);
            await this.RunMethodInController(controller, socketMessage, message);
        }

        public async Task Rooms(SocketMessage socketMessage)
        {
            var message = socketMessage.Content.TrimStart("/room ".ToCharArray());

            var controller = new RoomsController(socketMessage);
            await this.RunMethodInController(controller, socketMessage, message);
        }

        private async Task RunMethodInController<T>(T controller, SocketMessage socketMessage, string message) where T : BaseController
        {
            try
            {
                var method = controller.GetType().GetTypeInfo().GetMethods()
                    .First(x => x.GetCustomAttribute<Command>().CommandMessage == message);
                await (Task) method.Invoke(controller, null);
            }
            catch
            {
                await ErrorMessages.SendNotFoundCommand(socketMessage);
            }
        }
    }

    public class Command : Attribute
    {
        public string CommandMessage { get; private set; }

        public Command(string command)
        {
            CommandMessage = command;
        }
    }
}
