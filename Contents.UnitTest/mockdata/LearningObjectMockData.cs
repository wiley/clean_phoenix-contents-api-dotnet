using Contents.Domain.LearningObject;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace Contents.UnitTest.MockData
{
    public static class LearningObjectMockData
    {
        public static List<LearningObject> GetLearningObjectListData()
        {
            List<LearningObject> learningObjects = new List<LearningObject>
            {
                GenerateLearningObject(Guid.Parse("3fa35f64-5717-4562-b3fc-2c963f66afa6")),
                GenerateLearningObject(Guid.Parse("c3008b5b-0a73-4def-8496-ea850f154d9a"))
            };

            return learningObjects;
        }

        public static LearningObject GetLearningObject(Guid id)
        {
            return GenerateLearningObject(id);
        }

        private static LearningObject GenerateLearningObject(Guid id)
        {
            return new LearningObject
            {

                Id = id,
                Type = "INTERACTIVE",
                Title = "My Title",
                Description = "My Description",
                EstimatedDuration = 4600,
                LanguageTag = "pt-BR",
                Version = 1.58,
                Runtime = "INTERACTIVE_SCORM",
                ReferenceCode = "REF001",
                Provider = "CUSTOMER",
                Tags = new List<string> { "Java", "C#", "Communication" },
                Authors = new List<string> { "Mark Scullard", "Frank Sinatra" },
                Metadata = new BsonDocument
                {
                    { "customFieldText", "customValue1" },
                    { "customFieldInt", 1 },
                    { "customFieldDouble", 1.1 },
                    { "customFieldObject", new BsonDocument { { "String1", "String2" } } }
                },
                ThumbnailPath = "path/to/thumnail.png",
                OrganizationIds = new List<int> { 534, 123, 654, 1092381 },
                ProductIds = new List<Guid> { id },
                IsDiscoverable = true,
                CreatedBy = 1231,
                UpdatedBy = 123019,
                CreatedAt = DateTime.Parse("2023-02-22T21:40:18.243Z"),
                UpdatedAt = DateTime.Parse("2023-02-22T21:40:18.243Z")
            };
        }
    }
}
