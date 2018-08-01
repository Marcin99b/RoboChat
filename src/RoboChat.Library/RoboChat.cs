using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newtonsoft.Json;
using RoboChat.Library.Settings;

namespace RoboChat.Library
{
    public class RoboChat
    {
        private readonly IMongoDatabase database;
        private IMongoCollection<HistoryMessage> messagesCollection => database.GetCollection<HistoryMessage>("RoboChatMessages");
        
        private readonly List<HistoryMessage> basedMessagesHistory;
        private List<HistoryMessage> currentSessionMessagesHistory;
        public IEnumerable<HistoryMessage> FullSessionMessagesHistory => basedMessagesHistory.mergeCollections(currentSessionMessagesHistory);

        private readonly string roboUsername;

        private readonly SmilarityProcessor smilarityProcessor;
        private readonly SessionSettings sessionSettings;
        
        public RoboChat(SessionSettings settings)
        {
            database = MongoConfigurator.Connect();
            smilarityProcessor = new SmilarityProcessor();
            sessionSettings = settings;
            currentSessionMessagesHistory = new List<HistoryMessage>();
            this.roboUsername = $"RoboChat";

            try
            {
                basedMessagesHistory = messagesCollection.FindSync(_ => true).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                basedMessagesHistory = new List<HistoryMessage>();
            }
        }

        public void MergeHistory()
        {
            messagesCollection.InsertMany(currentSessionMessagesHistory);
            currentSessionMessagesHistory = new List<HistoryMessage>();
        }

        public void DeleteSessionChat()
        {
            currentSessionMessagesHistory = new List<HistoryMessage>();
        }

        public string SendMessage(TextLine userMessage)
        {
            var lastMessages = currentSessionMessagesHistory.LastOrDefault();
            if(lastMessages != null)
            if (!lastMessages.UserResponsed)
            {
                lastMessages.SetUserResponse(userMessage);
            }
            
            var reaction = GetRoboChatReaction(userMessage);
            var currentMessage = new HistoryMessage(reaction, null);
            currentSessionMessagesHistory.Add(currentMessage);
            return reaction.Message;
        }

        private TextLine GetRoboChatReaction(TextLine userMessage)
        {
            var validSentences = new List<ValidSentence>();
            foreach (var message in FullSessionMessagesHistory.Where(x => x.UserResponsed))
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

        public int NumberOfMessagesInCurrentSession()
        {
            return currentSessionMessagesHistory.Count;
        }
    }
}
