using Contents.Domain.Pagination;
using Contents.Services;
using Contents.Services.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningObjectModel = Contents.Domain.LearningObject.LearningObject;

namespace Contents.UnitTest.Services
{
    public class PaginationServiceTest
    {
        private readonly IPaginationService<LearningObjectModel> _service;

        public PaginationServiceTest()
        {
            _service = new PaginationService<LearningObjectModel>();
        }

        [Test]
        public async Task PaginationTestAsync()
        {
            string[] stringArray = new string[] { "Test" };
            var filter = new Filter() { FieldName = "Title", Values = stringArray };
            var listFilter = new List<Filter>();
            listFilter.Add(filter);

            var pageRequest = new PageRequest()
            {
                PageOffset = 0,
                PageSize = 1,
                SortField = "Description",
                SortOrder = EnumSortOrder.Ascending,
                Filters = listFilter
            };

            var result = await _service.ApplyPaginationAsync(MockData.ForPaginationMockData.GetLearningObject().AsQueryable(), pageRequest);

            Assert.IsNotNull(result);
            Assert.AreEqual("Description", result.FirstOrDefault().Description);
        }

        [Test]
        public async Task PaginationNoFilterTestAsync()
        {
            var listFilter = new List<Filter>();
            var pageRequest = new PageRequest()
            {
                PageOffset = 0,
                PageSize = 1,
                SortField = "Description",
                SortOrder = EnumSortOrder.Ascending,
                Filters = listFilter
            };

            var result = await _service.ApplyPaginationAsync(MockData.ForPaginationMockData.GetLearningObject().AsQueryable(), pageRequest);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task PaginationSortDescendingTestAsync()
        {
            string[] stringArray = new string[] { "Test" };
            var filter = new Filter() { FieldName = "Title", Values = stringArray };
            var listFilter = new List<Filter>();
            listFilter.Add(filter);

            var pageRequest = new PageRequest()
            {
                PageOffset = 0,
                PageSize = 1,
                SortField = "Description",
                SortOrder = EnumSortOrder.Descending,
                Filters = listFilter
            };

            var result = await _service.ApplyPaginationAsync(MockData.ForPaginationMockData.GetLearningObject().AsQueryable(), pageRequest);

            Assert.IsNotNull(result);

            Assert.AreEqual("Description2", result.FirstOrDefault().Description);
        }

        [Test]
        public async Task PaginationOtherSortTestAsync()
        {
            string[] stringArray = new string[] { "Test" };
            var filter = new Filter() { FieldName = "Title", Values = stringArray };
            var listFilter = new List<Filter>();
            listFilter.Add(filter);

            var pageRequest = new PageRequest()
            {
                PageOffset = 0,
                PageSize = 1,
                SortField = "Description",
                SortOrder = (EnumSortOrder)2,
                Filters = listFilter
            };

            var result = await _service.ApplyPaginationAsync(MockData.ForPaginationMockData.GetLearningObject().AsQueryable(), pageRequest);

            Assert.IsNotNull(result);
        }
    }
}
