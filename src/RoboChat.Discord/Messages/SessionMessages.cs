using System.Threading.Tasks;
using Discord.WebSocket;
using RoboChat.Discord.Helpers;
using RoboChat.Library;
using Discord;
namespace RoboChat.Discord.Messages
{
    public static class SessionMessages
    {
        public static async Task SendResponseWithListOfCommands(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync($"Commands: \n" +
                                                         $"```/session -help (for all)\n" +
                                                         $"/session -author (for all)\n" +
                                                         $"/session -start (for all)\n" +
                                                         $"/session -start -learn faster (for all)\n" +
                                                         $"/session -delete (for admins and authors)\n" +
                                                         $"/session -ready (for authors)\n" +
                                                         $"/session -merge (for admins)\n" +
                                                         $"/room -clear (for all)\n" +
                                                         $"/bot or /b (for authors)```");
        }

        public static async Task NotFoundSession(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync($"```This room doesn't have any session. If you want to start a conversation, you need to write: /session -start```");
        }

        public static async Task CannotCreateSessionBecauseOtherUser(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync($"```Cannot create session in room: {socketMessage.Channel.Name}, because other user has session here```");
        }

        public static async Task CreatedSession(SocketMessage socketMessage, ChatSession session)
        {
            await socketMessage.Channel.SendMessageAsync($"```Created session for user: {session.SessionOwner} in room: {session.RoomName}```");
        }

        public static async Task DeletedSession(SocketMessage socketMessage, ChatSession session)
        {
            await socketMessage.Channel.SendMessageAsync($"```Deleted session for user: {session.SessionOwner} in room: {session.RoomName}```");
        }

        public static async Task MergedSession(SocketMessage socketMessage, ChatSession session)
        {
            await socketMessage.Channel.SendMessageAsync($"```Merged session for user: {session.SessionOwner} in room: {session.RoomName} by user: {Userhelper.GetFullUsername(socketMessage)}```");
        }

        public static async Task AuthorOfSession(SocketMessage socketMessage, ChatSession session)
        {
            await socketMessage.Channel.SendMessageAsync($"```Session in room: {session.RoomName} has author: {session.SessionOwner}```");
        }

        public static async Task CannotSetAsReadyToMerge(SocketMessage socketMessage, ChatSession session)
        {
            await socketMessage.Channel.SendMessageAsync($"```Cannot set as ready session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission```");
        }

        public static async Task ReadyToMergeSession(SocketMessage socketMessage, ChatSession session, IMessageChannel messageChannel)
        {
            await socketMessage.Channel.SendMessageAsync($"```Admins have been notified about the session with user: {session.SessionOwner} in room: {session.RoomName}```");
            await messageChannel.SendMessageAsync($"```Session with user: {session.SessionOwner} in room: {session.RoomName} is ready for merge```");
        }

        public static async Task LearnFasterModeActivated(SocketMessage socketMessage)
        {
            await socketMessage.Channel.SendMessageAsync($"```css\nLearn faster mode activated\n```");
        }

        public static async Task CannotMergeBcsPermission(SocketMessage socketMessage, ChatSession session)
        {
            await socketMessage.Channel.SendMessageAsync($"```Cannot merge session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission```");
        }

        public static async Task CannotMergeBcsEmptySession(SocketMessage socketMessage, ChatSession session)
        {
            await socketMessage.Channel.SendMessageAsync($"```Cannot merge session for user: {session.SessionOwner} in room: {session.RoomName}, because this session doesn't contain any message```");
        }

        public static async Task CannotDeleteSessionBcsPermission(SocketMessage socketMessage, ChatSession session)
        {
            await socketMessage.Channel.SendMessageAsync($"```Cannot delete session for user: {session.SessionOwner} in room: {session.RoomName}, because you haven't permission```");
        }
    }
}
