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

        public List<ChatSession> returnChatSessionList()
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

        private async Task<ChatSession> GetThisRoomChatSession(SocketMessage socketMessage)
        {
            var sessionToMerge = chatSessions.FirstOrDefault(x => x.RoomName == socketMessage.Channel.Name);

            if (sessionToMerge == null)
            {
                await SessionMessages.NotFoundSession(socketMessage);
                return null;
            }
            return sessionToMerge;
        }

        public async Task SendResponseWithSessionAuthor(SocketMessage socketMessage)
        {
            var session = await GetThisRoomChatSession(socketMessage);
            if (session == null)
            {
                return;
            }
            await socketMessage.Channel.SendMessageAsync($"```Session in room: {socketMessage.Channel.Name} has author: {session.SessionOwner}```");
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
                await socketMessage.Channel.SendMessageAsync($"```Cannot set as ready session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission```");
                return;
            }

            await socketMessage.Channel.SendMessageAsync($"```Admins have been notified about the session with user: {session.SessionOwner} in room: {session.RoomName}```");
            await messageChannel.SendMessageAsync($"```Session with user: {session.SessionOwner} in room: {session.RoomName} is ready for merge```");
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
                await socketMessage.Channel.SendMessageAsync($"```css\nLearn faster mode activated\n```");
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
                await socketMessage.Channel.SendMessageAsync($"```Cannot merge session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission```");
                return;
            }
            if (session.RoboChat.NumberOfMessagesInCurrentSession == 0)
            {
                await socketMessage.Channel.SendMessageAsync($"```Cannot merge session for user: {session.SessionOwner} in room: {session.RoomName}, because this session doesn't contain any message```");
                return;
            }
            
            session.RoboChat.MergeHistory();
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
                await socketMessage.Channel.SendMessageAsync($"```Cannot delete session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission```");
                return;
            }
            session.RoboChat.DeleteSessionChat();
            chatSessions.Remove(session);
            await SessionMessages.DeletedSession(socketMessage, session);

        }
    }
}
