using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using RoboChat.Discord.ConsoleDisplay;
using RoboChat.Discord.Helpers;
using RoboChat.Discord.Messages;
using RoboChat.Library;
using RoboChat.Library.Settings;

namespace RoboChat.Discord.Services
{
    public class SessionService
    {

        private static List<ChatSession> chatSessions = new List<ChatSession>();
        public IEnumerable<ChatSession> ChatSessions => chatSessions;

        public List<ChatSession> ReturnChatSessionList()
        {
            return chatSessions;
        }

        public async Task SendResponseToUser(SocketMessage socketMessage, string message)
        {
            var currentSession = chatSessions.Where(x => x.RoomName == socketMessage.Channel.Name)
                .FirstOrDefault(x => x.SessionOwner == Userhelper.GetFullUsername(socketMessage));
            if (currentSession == null)
            {
                await SessionMessages.NotFoundSession(socketMessage);
                return;
            }

            var response = currentSession.RoboChat.SendMessage(new TextLine(socketMessage.Author.Username, message));
            if (response.StartsWith("/"))
            {
                response = response.TrimStart("/".ToCharArray());
            }
            await socketMessage.Channel.SendMessageAsync(response);
            
            PrintToConsole.PrintMessages(socketMessage, message, response);
        }

        public static async Task<ChatSession> GetThisRoomChatSession(SocketMessage socketMessage)
        {
            var sessionToMerge = chatSessions.FirstOrDefault(x => x.RoomName == socketMessage.Channel.Name);

            if (sessionToMerge == null)
                return null;
            
            return sessionToMerge;
        }

        public async Task SendResponseWithSessionAuthor(SocketMessage socketMessage)
        {
            var session = await GetThisRoomChatSession(socketMessage);
            if (session == null)
            {
                await SessionMessages.NotFoundSession(socketMessage);
                return;
            }
            await SessionMessages.AuthorOfSession(socketMessage, session);
        }
        
        public async Task ReadyToMerge(SocketMessage socketMessage, IMessageChannel messageChannel)
        {
            var session = await GetThisRoomChatSession(socketMessage);
            if (session == null)
            {
                return;
            }

            if (session.SessionOwner != Userhelper.GetFullUsername(socketMessage) && !Userhelper.IsAdmin(socketMessage))
            {
                await SessionMessages.CannotSetAsReadyToMerge(socketMessage, session);
                return;
            }

            await SessionMessages.ReadyToMergeSession(socketMessage, session, messageChannel);
        }

        public async Task CreateNewSession(SocketMessage socketMessage)
        {

            var learnFaster = socketMessage.Content.Contains(" -learn faster");
            
            var settings = new SessionSettings($"{Userhelper.GetFullUsername(socketMessage)}--{socketMessage.Channel.Name}", learnFaster);
            var newSession = new ChatSession(Userhelper.GetFullUsername(socketMessage), socketMessage.Channel.Name, settings);
            chatSessions.Add(newSession);

            await SessionMessages.CreatedSession(socketMessage, newSession);
            if (learnFaster)
            {
                await SessionMessages.LearnFasterModeActivated(socketMessage);
            }
        }

        public async Task MergeSession(SocketMessage socketMessage)
        {
            var session = await GetThisRoomChatSession(socketMessage);
            if (session == null)
            {
                return;
            }
            if (!Userhelper.IsAdmin(socketMessage))
            {
                await SessionMessages.CannotMergeBcsPermission(socketMessage, session);
                return;
            }
            if (session.RoboChat.NumberOfMessagesInCurrentSession == 0)
            {
                await SessionMessages.CannotMergeBcsEmptySession(socketMessage, session);
                return;
            }
            
            session.RoboChat.MergeSession();
            await SessionMessages.MergedSession(socketMessage, session);
        }

        public async Task DeleteSession(SocketMessage socketMessage)
        {
            var session = await GetThisRoomChatSession(socketMessage);
            if (session == null)
            {
                return;
            }
            if (session.SessionOwner != Userhelper.GetFullUsername(socketMessage) && !Userhelper.IsAdmin(socketMessage))
            {
                await SessionMessages.CannotDeleteSessionBcsPermission(socketMessage, session);
                return;
            }
            session.RoboChat.DeleteSession();
            chatSessions.Remove(session);
            await SessionMessages.DeletedSession(socketMessage, session);

        }
    }
}
