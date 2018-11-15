using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newtonsoft.Json;
using RoboChat.Library.Conversation;
using RoboChat.Library.Session;
using RoboChat.Library.Settings;

namespace RoboChat.Library
{
    public class RoboChat
    {
        private readonly SessionSettings sessionSettings;
        private readonly SessionService sessionService;
        private readonly ConversationService conversationService;
        
        public RoboChat(SessionSettings settings)
        {
            sessionSettings = settings;
            this.sessionService = new SessionService();
            this.conversationService = new ConversationService(sessionSettings, "RoboChat");
        }

        public void MergeHistory()
        {
            sessionService.Merge();
        }

        public void DeleteSessionChat()
        {
            sessionService.Delete();
        }

        public string SendMessage(TextLine userMessage)
        {
            var lastMessage = sessionService.CurrentSessionMessagesHistory.LastOrDefault();
            var botReaction = conversationService.GetBotResponse(userMessage, sessionService.FullSessionMessagesHistory, lastMessage);
            sessionService.AddBotMessage(botReaction);

            return botReaction.RobotMessage.Message;
        }
    }
}
