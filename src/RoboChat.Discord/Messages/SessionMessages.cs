using System.Threading.Tasks;
using Discord.WebSocket;
using RoboChat.Discord.Helpers;
using RoboChat.Library;

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
    }
}
