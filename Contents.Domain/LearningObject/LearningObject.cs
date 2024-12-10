using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contents.Domain.LearningObject
{
    public class MetadataConverter : JsonConverter<BsonDocument>
    {
        private JArray WalkArray(BsonDocument root)
        {
            var result = new JArray();
            foreach (var rootElement in root.Elements.Where(el => el.Name == "Value"))
            {
                foreach (var item in rootElement.Value.AsBsonArray)
                {
                    var type = item.GetType();
                    if (type == typeof(BsonDocument))
                    {
                        result.Add(Walk(item.ToBsonDocument()));
                        continue;
                    }

                    if (type == typeof(BsonArray))
                    {
                        result.Add(WalkArray(item.ToBsonDocument()));
                        continue;
                    }

                    if (type == typeof(BsonNull))
                    {
                        result.Add(JValue.CreateNull());
                        continue;
                    }

                    result.Add(JToken.FromObject(item));
                }
            }

            return result;
        }

        private JObject Walk(BsonDocument value)
        {
            var result = new JObject();
            foreach (var item in value.Elements)
            {
                var type = item.Value.GetType();
                if (type == typeof(BsonDocument))
                {
                    result.Add(item.Name, Walk(item.Value.ToBsonDocument()));
                    continue;
                }

                if (type == typeof(BsonArray))
                {
                    result.Add(item.Name, WalkArray(item.ToBsonDocument()));
                    continue;
                }

                if (type == typeof(BsonNull))
                {
                    result.Add(item.Name, JValue.CreateNull());
                    continue;
                }

                result.Add(item.Name, JToken.FromObject(item.Value));
            }
            return result;
        }

        public override void WriteJson(JsonWriter writer, BsonDocument value, JsonSerializer serializer)
        {
            var obj = Walk(value);
            obj.WriteTo(writer);
        }

        public override BsonDocument ReadJson(JsonReader reader, Type objectType, BsonDocument existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => false;
    }

    [BsonCollection("learning_objects")]
    [BsonIgnoreExtraElements]
    public class LearningObject : GenericEntity
    {
        [BsonIgnoreIfNull]
        public string Type { get; set; }

        [BsonIgnoreIfNull]
        public string Title { get; set; }

        [BsonIgnoreIfNull]
        public string Description { get; set; }

        [BsonIgnoreIfNull]
        public int? EstimatedDuration { get; set; }

        [BsonIgnoreIfNull]
        public string LanguageTag { get; set; }

        [BsonIgnoreIfNull]
        public double? Version { get; set; }

        [BsonIgnoreIfNull]
        public string Runtime { get; set; }

        [BsonIgnoreIfNull]
        public string ReferenceCode { get; set; }

        [BsonIgnoreIfNull]
        public string Provider { get; set; }

        [BsonIgnoreIfNull]
        public List<string> Tags { get; set; }

        [BsonIgnoreIfNull]
        public List<string> Authors { get; set; }

        [BsonIgnoreIfNull]
        [JsonConverter(typeof(MetadataConverter))]
        public BsonDocument Metadata { get; set; }

        [BsonIgnoreIfNull]
        public string ThumbnailPath { get; set; }

        [BsonIgnoreIfNull]
        public List<int> OrganizationIds { get; set; }

        [BsonIgnoreIfNull]
        public List<Guid> ProductIds { get; set; }

        [BsonIgnoreIfNull]
        public bool IsDiscoverable { get; set; }
    }
}
