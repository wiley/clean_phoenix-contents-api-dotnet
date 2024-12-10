using Contents.Domain.LearningObject;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Contents.API.Responses.LearningObject
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MetadataResponse
    {
        public Filepaths Filepaths { get; set; }

        public string Filepath { get; set; }

        public string Url { get; set; }
    }
}
