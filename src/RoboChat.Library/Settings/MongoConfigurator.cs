using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace RoboChat.Library.Settings
{
    public static class MongoConfigurator
    {
        private static bool _initialized;
        
        public static IMongoDatabase Connect()
        {
            var connectionString = new MongoUrl("");
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
