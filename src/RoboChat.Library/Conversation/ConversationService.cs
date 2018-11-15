using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoboChat.Library.Settings;

namespace RoboChat.Library.Conversation
{
    class ConversationService
    {
        private readonly SessionSettings sessionSettings;
        private readonly string roboUsername;
        private readonly SmilarityProcessor smilarityProcessor;

        public ConversationService(SessionSettings sessionSettings, string roboUsername)
        {
            this.sessionSettings = sessionSettings;
            this.roboUsername = roboUsername;
            this.smilarityProcessor = new SmilarityProcessor();
        }

        public HistoryMessage GetBotResponse(TextLine userMessage, IEnumerable<HistoryMessage> messages, HistoryMessage lastMessage)
        {
            var lastMessages = lastMessage;
            if (lastMessages != null)
                if (!lastMessages.UserResponsed)
                {
                    lastMessages.SetUserResponse(userMessage);
                }

            var reaction = this.GetRoboChatReaction(userMessage, messages);
            var currentMessage = new HistoryMessage(reaction, null);
            return currentMessage;
        }

        private TextLine GetRoboChatReaction(TextLine userMessage, IEnumerable<HistoryMessage> sessionMessages)
        {
            var validSentences = new List<ValidSentence>();
            foreach (var message in sessionMessages.Where(x => x.UserResponsed))
            {
                smilarityProcessor.GetSimilarityRatio(message.RobotMessage.Message, userMessage.Message, out double ratio, out double realRatio);
                if (message.RobotMessage.Message == userMessage.Message)
                {
                    validSentences.Add(new ValidSentence
                    {
                        Smilarity = Int32.Parse(ratio.ToString()),
                        Message = message.UserResponse.Message,
                        TheSame = true
                    });
                }
                else if (ratio >= 70 && !validSentences.Any(x => x.TheSame) && !sessionSettings.LearnFaster)
                {
                    validSentences.Add(new ValidSentence
                    {
                        Smilarity = Int32.Parse(ratio.ToString()),
                        Message = message.UserResponse.Message,
                        TheSame = true
                    });
                }
            }
            var roboMessage = validSentences.Any() ? GetRandomMessage(validSentences) : userMessage.Message;
            return new TextLine(roboUsername, roboMessage);
        }

        private string GetRandomMessage(IEnumerable<ValidSentence> validSentences)
        {
            return validSentences.Skip(new Random().Next(validSentences.Count())).First().Message;
        }
    }
}
