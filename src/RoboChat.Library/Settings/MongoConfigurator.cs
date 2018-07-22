using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace RoboChat.Library
{
    public static class MongoConfigurator
    {
        private static bool _initialized;
        
        public static IMongoDatabase Connect()
        {
            //var connectionString = new MongoUrl("mongodb://chathistoryuser:eaugfb43eubagfb431naenfaei@ds237120.mlab.com:37120/robochat");
            var connectionString = new MongoUrl("mongodb://user:SGWStUy9hnMgE5PV5CWqw4zuZmTcASnb3Bwe3aRzBmL9Yh@ds229388.mlab.com:29388/hated");
            MongoClientSettings settings = new MongoClientSettings
            {
                Credential = MongoCredential.CreateCredential("hated", "user", "SGWStUy9hnMgE5PV5CWqw4zuZmTcASnb3Bwe3aRzBmL9Yh"),
                Server = new MongoServerAddress(connectionString.ToString(), 29388)
            };
            MongoClient client = new MongoClient(connectionString.ToString());
            return client.GetDatabase(connectionString.DatabaseName);
        }

        private static void RegisterConventions()
        {
            ConventionRegistry.Register("RoboChatConventions", new MongoConventions(), x => true);
        }

        private class MongoConventions : IConventionPack
        {
            public IEnumerable<IConvention> Conventions => new List<IConvention>
            {
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
                new CamelCaseElementNameConvention()
            };
        }
    }
}
