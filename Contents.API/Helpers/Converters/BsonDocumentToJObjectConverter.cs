using AutoMapper;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace Contents.API.Helpers.Converters
{
    public class BsonDocumentToJObjectConverter : ITypeConverter<BsonDocument, JObject>
    {
        public JObject Convert(BsonDocument source, JObject destination, ResolutionContext context)
        {
            if (source != null)
            {
                var jObject = JObject.Parse(source.ToJson());
                return jObject;
            }
            return null;
        }
    }
}
