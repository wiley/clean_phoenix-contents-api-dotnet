using AutoMapper;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace Contents.API.Helpers.Converters
{
    public class JObjectToBsonDocumentConverter : ITypeConverter<JObject, BsonDocument>
    {
        public BsonDocument Convert(JObject source, BsonDocument destination, ResolutionContext context)
        {
            if (source != null)
                return BsonDocument.Parse(source.ToString());
            return null;
        }
    }
}
