using Contents.Infrastructure.Interface.Mongo;

namespace Contents.Infrastructure.Mongo
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}