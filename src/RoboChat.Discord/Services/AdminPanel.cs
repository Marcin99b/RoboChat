using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoboChat.Library;

namespace RoboChat.Discord.Services
{
    class AdminPanel
    {
        private bool isDebugMode;
        private List<ChatSession> sessionsList = new List<ChatSession>();

        public AdminPanel(List<ChatSession> service, bool isDebugMode)
        {
            this.isDebugMode = isDebugMode;
            sessionsList = service;
        }

        public void CheckCommand(SocketMessage socketMessage)
        {
            string message = socketMessage.ToString();
            if (isDebugMode != message.StartsWith("."))
                return;
            message = message.TrimStart("./".ToArray());

            switch(message)
            {
                case "help":
                    SendResponseWithListOfCommands(socketMessage);
                    break;

                case "session -count":
                case "sc":
                    SendResponseWithNumberOfSessions(socketMessage);
                    break;

                case "session -list":
                case "sl":
                    SendResponseWithActiveSessions(socketMessage, true);
                    break;

                case "base -messages":

                    break;

                default: // Commands which cannot be checked with switch - commands contains numbers
                    if (message.Contains("session -messages") || message.Contains("sm"))
                        SendResponseWithSessionAllMessages(socketMessage);
                    break;
            }
        }

        public async void SendResponseWithListOfCommands(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync("Commands: \n" +
                                                         "```/help\n" +
                                                         "/session -count (/sc)\n" +
                                                         "/session -list (/sl)\n" +
                                                         "/session -messages <number> (/sm <number>)\n" +
                                                         "/base -messages```");
        }

        public async void SendResponseWithNumberOfSessions(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync($"There are {sessionsList.Count} sessions in bot-talk rooms at the moment\n");
        }

        public /*async*/ Boolean SendResponseWithActiveSessions(SocketMessage socketMessage, bool showActiveSessionsList)
        {
            if (sessionsList.Count == 0)
            {
                socketMessage.Channel.SendMessageAsync("There isn't any session now");
                return false;
            }

            if (showActiveSessionsList == false)
                return true;

            socketMessage.Channel.SendMessageAsync("Active sessions now:\n");
            string allLinesToSend = "```";
            int i = 0;

            foreach (ChatSession singleSession in sessionsList)
                allLinesToSend += ++i + ": " + "Room: #" + singleSession.RoomName + "  User: " + singleSession.SessionOwner + "  Messages: " + singleSession.RoboChat.NumberOfMessagesInCurrentSession + "\n";
            allLinesToSend += "```";

            socketMessage.Channel.SendMessageAsync(allLinesToSend);
            return true;
        }

        public async void SendResponseWithSessionAllMessages(SocketMessage socketMessage)
        {
            if (!SendResponseWithActiveSessions(socketMessage, false))
                return;

            string numberOfSessionString = socketMessage.ToString().TrimStart("./session -messages".ToArray());
            
            int numberOfSession;
            if (Int32.TryParse(numberOfSessionString, out numberOfSession) == false)
                return;

            if (--numberOfSession >= sessionsList.Count || numberOfSession < 0)
                return;

            string allLinesToSend = "```";

            ChatSession singleSession = sessionsList[numberOfSession];
            List<HistoryMessage> messages = singleSession.RoboChat.returnSessionMessagesHistory;

            if (messages.Count == 0)
            {
                allLinesToSend += "There aren't any message to show```";
                await socketMessage.Channel.SendMessageAsync(allLinesToSend);
                return;
            }

            for (int i = 0; i < singleSession.RoboChat.NumberOfMessagesInCurrentSession - 1; i++)
            {
                allLinesToSend += "\n";
                allLinesToSend += "Bot: " + messages.ElementAt(i).RobotMessage.Message + "\n";
                allLinesToSend += "User: " + messages.ElementAt(i).UserResponse.Message + "\n";
            }
            allLinesToSend += "\n" + "Bot: " + messages.ElementAt(messages.Count - 1).RobotMessage.Message;
            allLinesToSend += "```";

            await socketMessage.Channel.SendMessageAsync($"All messages in room: {singleSession.RoomName}");
            await socketMessage.Channel.SendMessageAsync(allLinesToSend);
        }
    }
}
