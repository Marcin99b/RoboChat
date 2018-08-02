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
        //private readonly string sessionsFileName = @"Sessions.txt";

        public SessionService()
        {

            /*if (!File.Exists(sessionsFileName))
            {
                try
                {
                    File.Create(sessionsFileName).Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            var sessions = JsonConvert.DeserializeObject<List<ChatSession>>(File.ReadAllText(sessionsFileName));
            if (sessions != null)
            {
                chatSessions = sessions;
            }
            
            */
            chatSessions = new List<ChatSession>();
            
        }

        public async Task SendResponseToUser(SocketMessage socketMessage)
        {
            var currentSssion = chatSessions.Where(x => x.RoomName == socketMessage.Channel.Name)
                .FirstOrDefault(x => x.SessionOwner == GetFullUsername(socketMessage));
            if (currentSssion == null)
            {
                await socketMessage.Channel.SendMessageAsync($"Cannot search session for user: {GetFullUsername(socketMessage)} in room: {socketMessage.Channel.Name}");
                return;
            }

            var message = socketMessage.ToString().TrimStart("/bot".ToCharArray());
            if (message.StartsWith(" "))
            {
                message = message.TrimStart(" ".ToCharArray());
            }

            var response = currentSssion.RoboChat.SendMessage(new TextLine(socketMessage.Author.Username, message));
            if (response.StartsWith("/"))
            {
                response = response.TrimStart("/".ToCharArray());
            }
            await socketMessage.Channel.SendMessageAsync(response);
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nFrom: {GetFullUsername(socketMessage)}");
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
                await socketMessage.Channel.SendMessageAsync($"Cannot search session  in room: {socketMessage.Channel.Name}");
                return null;
            }
            return sessionToMerge;
        }

        public async Task SendResponseWithSessionAuthor(SocketMessage socketMessage)
        {
            var session = await GetThisRoomChatSession(socketMessage);
            if (session == null)
            {
                await socketMessage.Channel.SendMessageAsync($"This room is empty, you can create session: /start-session");
                return;
            }
            await socketMessage.Channel.SendMessageAsync($"Session in room: {socketMessage.Channel.Name} has author: {session.SessionOwner}");
        }

        public async Task SendResponseWithListOfCommands(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync($"Commands: \n" +
                                                         $"```/session -help (for all)\n" +
                                                         $"/session -author (for all)\n" +
                                                         $"/session -start (for all)\n" +
                                                         $"/session -start -learn faster (for all)\n" +
                                                         $"/session -delete (for admins and authors)\n" +
                                                         $"/session -ready (for authors)\n" +
                                                         $"/session -merge (for admins)\n" +
                                                         $"/bot (for authors)```");
        }

        public async Task SendResponseWithLoading(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync($"**Loading... It may take a few seconds.**");
        }

        public async Task SendResponseWithInfoAboutOffline(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync("**If bot \"RoboChat\" is offline, bot is not working.**");
        }

        public async Task ReadyToMerge(SocketMessage socketMessage, IMessageChannel messageChannel)
        {
            var session = await GetThisRoomChatSession(socketMessage);
            if (session == null)
            {
                return;
            }

            if (session.SessionOwner != GetFullUsername(socketMessage) && !IsAdmin(socketMessage))
            {
                await socketMessage.Channel.SendMessageAsync($"Cannot set as ready session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission");
                return;
            }

            await socketMessage.Channel.SendMessageAsync($"Admins have been notified about the session with user: {session.SessionOwner} in room: {session.RoomName}");
            await messageChannel.SendMessageAsync($"```Session with user: {session.SessionOwner} in room: {session.RoomName} is ready for merge```");
        }

        public async Task CreateNewSession(SocketMessage socketMessage)
        {
            if (chatSessions.Any(x => x.RoomName == socketMessage.Channel.Name))
            {
                await socketMessage.Channel.SendMessageAsync($"Cannot create session in room: {socketMessage.Channel.Name}, because other user has session here");
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
                return;
            }
            if (!IsAdmin(socketMessage))
            {
                await socketMessage.Channel.SendMessageAsync($"Cannot merge session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission");
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
                return;
            }
            if (session.SessionOwner != GetFullUsername(socketMessage) && !IsAdmin(socketMessage))
            {
                await socketMessage.Channel.SendMessageAsync($"Cannot delete session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission");
                return;
            }
            await SendResponseWithLoading(socketMessage);
            session.RoboChat.DeleteSessionChat();
            chatSessions.Remove(session);
            await socketMessage.Channel.SendMessageAsync($"```Deleted session for user: {session.SessionOwner} in room: {session.RoomName}```");

            await DeleteMessages(socketMessage);
            await SendResponseWithListOfCommands(socketMessage);
            await SendResponseWithInfoAboutOffline(socketMessage);
            UpdateSessionsFile();
        }

        public async Task DeleteMessages(SocketMessage socketMessage)
        {
            var channel = socketMessage.Channel;
            var messages = await channel.GetMessagesAsync().Flatten();
            await channel.DeleteMessagesAsync(messages);
        }
        
        public async Task ClearRoom(SocketMessage socketMessage)
        {
            await DeleteMessages(socketMessage);
            await SendResponseWithListOfCommands(socketMessage);
            await SendResponseWithInfoAboutOffline(socketMessage);
        }

        private void UpdateSessionsFile()
        {
            //File.WriteAllText(sessionsFileName, JsonConvert.SerializeObject(chatSessions));
        }

        private string GetFullUsername(SocketMessage socketMessage)
            => $"{socketMessage.Author.Username}#{socketMessage.Author.Discriminator}";

        private bool IsAdmin(SocketMessage message)
            => ((SocketGuildUser)message.Author).Roles.Any(role => role.Name == "administracja");
    }
}
