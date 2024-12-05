using Contents.Infrastructure.Interface.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Contents.Infrastructure.Mongo
{
    public class MongoTestConnection : IMongoTestConnection
    {
        private readonly IMongoClient _client;
        private readonly IMongoDBSettings _settings;

        public MongoTestConnection(IMongoDBSettings settings, IMongoClient client)
        {
            _client = client;
            _settings = settings;
        }

        public bool Test()
        {
            var database = _client.GetDatabase(_settings.DatabaseName);

            // First execution may take a few seconds, so, I setup the wait for 5 seconds
            var isAlive = database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(5000);

            return isAlive;
        }
    }
}