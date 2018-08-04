using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using RoboChat.Library;
using RoboChat.Library.Settings;

namespace RoboChat.Discord.Services
{
    public class SessionService
    {

        private static List<ChatSession> chatSessions = new List<ChatSession>();
        

        public List<ChatSession> returnChatSessionList()
        {
            return chatSessions;
        }

        public async Task SendResponseToUser(SocketMessage socketMessage)
        {
            var currentSession = chatSessions.Where(x => x.RoomName == socketMessage.Channel.Name)
                .FirstOrDefault(x => x.SessionOwner == GetFullUsername(socketMessage));
            if (currentSession == null)
            {
                await socketMessage.Channel.SendMessageAsync($"```This room doesn't have any session. If you want to start a conversation, you need to write: /session -start```");
                return;
            }

            var message = socketMessage.ToString().TrimStart("/bot".ToCharArray());
            if (message.StartsWith(" "))
            {
                message = message.TrimStart(" ".ToCharArray());
            }

            var response = currentSession.RoboChat.SendMessage(new TextLine(socketMessage.Author.Username, message));
            if (response.StartsWith("/"))
            {
                response = response.TrimStart("/".ToCharArray());
            }
            await socketMessage.Channel.SendMessageAsync(response);
            
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

        public async Task<ChatSession> GetThisRoomChatSession(SocketMessage socketMessage)
        {
            var sessionToMerge = chatSessions.FirstOrDefault(x => x.RoomName == socketMessage.Channel.Name);

            if (sessionToMerge == null)
            {
                //await socketMessage.Channel.SendMessageAsync($"Cannot search session in room: {socketMessage.Channel.Name}");
                return null;
            }
            return sessionToMerge;
        }

        public async Task SendResponseWithSessionAuthor(SocketMessage socketMessage)
        {
            var session = await GetThisRoomChatSession(socketMessage);
            if (session == null)
            {
                await socketMessage.Channel.SendMessageAsync($"```This room is empty, you can create a new session: /start-session```");
                return;
            }
            await socketMessage.Channel.SendMessageAsync($"```Session in room: {socketMessage.Channel.Name} has author: {session.SessionOwner}```");
        }

        

        public async Task SendResponseWithInfoAboutOffline(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync("***```If bot \"RoboChat\" is offline, bot is not working!```***");
        }

        public async Task ReadyToMerge(SocketMessage socketMessage, IMessageChannel messageChannel)
        {
            var session = await GetThisRoomChatSession(socketMessage);
            if (session == null)
            {
                await socketMessage.Channel.SendMessageAsync($"```There is no session in this room, you can create a new session: /start-session```");
                return;
            }

            if (session.SessionOwner != GetFullUsername(socketMessage) && !IsAdmin(socketMessage))
            {
                await socketMessage.Channel.SendMessageAsync($"```Cannot set as ready session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission```");
                return;
            }

            await socketMessage.Channel.SendMessageAsync($"```Admins have been notified about the session with user: {session.SessionOwner} in room: {session.RoomName}```");
            await messageChannel.SendMessageAsync($"```Session with user: {session.SessionOwner} in room: {session.RoomName} is ready for merge```");
        }

        public async Task CreateNewSession(SocketMessage socketMessage)
        {
            if (chatSessions.Any(x => x.RoomName == socketMessage.Channel.Name))
            {
                await socketMessage.Channel.SendMessageAsync($"```Cannot create session in room: {socketMessage.Channel.Name}, because other user has session here```");
                return;
            }

            await DeleteMessages(socketMessage);
            await SendResponseWithListOfCommands(socketMessage);
            await SendResponseWithLoading(socketMessage);

            var learnFaster = socketMessage.Content.Contains(" -learn faster");
            
            var settings = new SessionSettings($"{GetFullUsername(socketMessage)}--{socketMessage.Channel.Name}", learnFaster);
            var newSession = new ChatSession(GetFullUsername(socketMessage), socketMessage.Channel.Name, settings);
            chatSessions.Add(newSession);
            await socketMessage.Channel.SendMessageAsync($"```Created session for user: {newSession.SessionOwner} in room: {newSession.RoomName}```");

            if (learnFaster)
            {
                await socketMessage.Channel.SendMessageAsync($"```css\nLearn faster mode activated\n```");
            }


            UpdateSessionsFile();
        }

        public async Task MergeSession(SocketMessage socketMessage)
        {
            var session = await GetThisRoomChatSession(socketMessage);
            if (session == null)
            {
                await socketMessage.Channel.SendMessageAsync($"```There is no session in this room, you can create a new session: /start-session```");
                return;
            }
            if (!IsAdmin(socketMessage))
            {
                await socketMessage.Channel.SendMessageAsync($"```Cannot merge session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission```");
                return;
            }
            if (session.RoboChat.NumberOfMessagesInCurrentSession == 0)
            {
                await socketMessage.Channel.SendMessageAsync($"```Cannot merge session for user: {session.SessionOwner} in room: {session.RoomName}, because this session doesn't contain any message```");
                return;
            }

            await SendResponseWithLoading(socketMessage);
            session.RoboChat.MergeHistory();
            await socketMessage.Channel.SendMessageAsync($"```Merged session for user: {session.SessionOwner} in room: {session.RoomName} by user: {GetFullUsername(socketMessage)}```");
        }

        public async Task DeleteSession(SocketMessage socketMessage)
        {
            var session = await GetThisRoomChatSession(socketMessage);
            if (session == null)
            {
                await socketMessage.Channel.SendMessageAsync($"```There is no session in this room, you can create a new session: /start-session```");
                return;
            }
            if (session.SessionOwner != GetFullUsername(socketMessage) && !IsAdmin(socketMessage))
            {
                await socketMessage.Channel.SendMessageAsync($"```Cannot delete session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission```");
                return;
            }
            session.RoboChat.DeleteSessionChat();
            chatSessions.Remove(session);
            await socketMessage.Channel.SendMessageAsync($"```Deleted session for user: {session.SessionOwner} in room: {session.RoomName}```");

            await DeleteMessages(socketMessage);
            await SendResponseWithListOfCommands(socketMessage);
            await SendResponseWithInfoAboutOffline(socketMessage);
            UpdateSessionsFile();
        }
        

        private string GetFullUsername(SocketMessage socketMessage)
            => $"{socketMessage.Author.Username}#{socketMessage.Author.Discriminator}";

        private bool IsAdmin(SocketMessage message)
            => ((SocketGuildUser)message.Author).Roles.Any(role => role.Name == "administracja");
    }
}
