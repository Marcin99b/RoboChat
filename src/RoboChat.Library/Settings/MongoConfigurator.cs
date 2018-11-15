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
            var connectionString = new MongoUrl("mongodb://user:user123@ds225703.mlab.com:25703/trackmymoney");
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
