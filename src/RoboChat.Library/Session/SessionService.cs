using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace RoboChat.Library.Session
{
    class SessionService
    {
        private readonly IMongoDatabase database;
        private IMongoCollection<HistoryMessage> messagesCollection => database.GetCollection<HistoryMessage>("RoboChatMessages");

        private readonly List<HistoryMessage> basedMessagesHistory;
        public List<HistoryMessage> CurrentSessionMessagesHistory;

        public IEnumerable<HistoryMessage> FullSessionMessagesHistory => basedMessagesHistory.withCollection(CurrentSessionMessagesHistory);

        public SessionService()
        {
            this.CurrentSessionMessagesHistory = new List<HistoryMessage>();

            database = MongoConfigurator.Connect();
            try
            {
                basedMessagesHistory = messagesCollection.FindSync(_ => true).ToList();
            }
            catch (Exception e)
            {
                basedMessagesHistory = new List<HistoryMessage>();
            }
        }

        public void AddBotMessage(HistoryMessage message)
        {
            CurrentSessionMessagesHistory.Add(message);
        }

        public void Merge()
        {
            messagesCollection.InsertMany(CurrentSessionMessagesHistory);
            CurrentSessionMessagesHistory = new List<HistoryMessage>();
        }

        public void Delete()
        {
            CurrentSessionMessagesHistory = new List<HistoryMessage>();
        }
    }
}
