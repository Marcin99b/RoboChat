using System.Threading.Tasks;
using Discord.WebSocket;

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

    }
}
