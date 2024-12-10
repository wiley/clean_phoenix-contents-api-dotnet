using System;
using System.Collections.Generic;
using LearningObjectModel = Contents.Domain.LearningObject.LearningObject;

namespace Contents.UnitTest.MockData
{
    public class ForPaginationMockData
    {
        private static readonly LearningObjectModel item = new()
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Description = "Description"
        };

        private static readonly LearningObjectModel item1 = new()
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Description = "Description1"
        };

        private static readonly LearningObjectModel item2 = new()
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Description = "Description2"
        };

        public static MongoDB.Driver.Linq.IMongoQueryable<LearningObjectModel> GetLearningObject()
        {
            var profile = new List<LearningObjectModel>();
            profile.Add(item);
            profile.Add(item1);
            profile.Add(item2);

            return new MongoQueryable<LearningObjectModel>() { MockData = profile };
        }
    }
}
