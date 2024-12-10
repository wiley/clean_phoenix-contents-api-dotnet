using LearningObjectModel = Contents.Domain.LearningObject.LearningObject;
using Contents.Domain.Pagination;
using Contents.Infrastructure.Interface.Mongo;
using Contents.Services;
using Contents.Services.Interfaces;
using Contents.UnitTest.MockData;
using DarwinAuthorization.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contents.Domain.LearningObject;
using MongoDB.Driver;
using System.Threading;
using Contents.UnitTest.Utils;

namespace Contents.UnitTest.Services.LearningObject
{
    public class LearningObjectServiceTest
    {
        private ILearningObjectService _service;
        private readonly Mock<IMongoRepository<LearningObjectModel>> _repository;
        private readonly Mock<IPaginationService<LearningObjectModel>> _paginationService;
        private readonly Mock<ILogger<LearningObjectService>> _logger;
        private readonly Mock<DarwinAuthorizationContext> _authorizationContext;

        public LearningObjectServiceTest()
        {
            _repository = new Mock<IMongoRepository<LearningObjectModel>>();
            _logger = new Mock<ILogger<LearningObjectService>>();
            _paginationService = new Mock<IPaginationService<LearningObjectModel>>();
            _authorizationContext = new Mock<DarwinAuthorizationContext>();
        }

        [Test]
        public async Task GetAll()
        {
            List<LearningObjectModel> mock = LearningObjectMockData.GetLearningObjectListData();
            LearningObjectFilter filter = new()
            {
                Offset = 0,
                Size = 20,
                EstimatedDurationEqual = 0,
                EstimatedDurationGreaterThan = 0,
                EstimatedDurationGreaterThanEqual = 0,
                EstimatedDurationLowerThan = 0,
                EstimatedDurationLowerThanEqual = 0,
                LanguageTag = "pt-BR",
                Type = "VIDEO"
            };

            _paginationService.Setup(r => r.ApplyPaginationAsync(It.IsAny<IQueryable<LearningObjectModel>>(), It.IsAny<PageRequest>())).Returns(Task.FromResult(mock));
            _service = new LearningObjectService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);
            
            List<LearningObjectModel> learningObjects = await _service.GetAll(filter);
            CollectionAssert.AreEqual(learningObjects, mock);
        }

        
        [Test]
        public void Get()
        {
            LearningObjectModel mock = LearningObjectMockData.GetLearningObject(Guid.NewGuid());

            _repository.Setup(r => r.FindById(It.IsAny<Guid>())).Returns(mock);
            _service = new LearningObjectService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);

            LearningObjectModel learningObject = _service.Get(mock.Id);
            Assert.AreEqual(learningObject, mock);
        }
        
        [Test]
        public async Task Create()
        {
            LearningObjectModel mock = LearningObjectMockData.GetLearningObject(Guid.NewGuid());

            _repository.Setup(r => r.InsertOneAsync(It.IsAny<LearningObjectModel>())).Returns(Task.FromResult(mock));
            _service = new LearningObjectService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);

            await _service.Create(mock);
            Assert.AreEqual(mock, mock);
        }

        
        [Test]
        public async Task Update()
        {
            LearningObjectModel mock = LearningObjectMockData.GetLearningObject(Guid.NewGuid());

            mock.Title = "Updated";
            mock.Description = "Updated";

            LearningObjectModel updateLearningObject = mock;

            _repository.Setup(r => r.ReplaceOneAsync(It.IsAny<LearningObjectModel>())).Returns(Task.FromResult(updateLearningObject));
            _service = new LearningObjectService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);

            await _service.Update(mock.Id, updateLearningObject);
            Assert.True(mock.Title == updateLearningObject.Title);
            Assert.True(mock.Description == updateLearningObject.Description);
            Assert.GreaterOrEqual(updateLearningObject.UpdatedAt, mock.UpdatedAt);
        }
        
        [Test]
        public void Delete()
        {
            LearningObjectModel mock = LearningObjectMockData.GetLearningObject(Guid.NewGuid());

            _repository.Setup(r => r.DeleteById(It.IsAny<Guid>())).Returns(mock);
            _service = new LearningObjectService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);

            LearningObjectModel learningObject = _service.Delete(mock.Id);
            Assert.AreEqual(learningObject, mock);
        }

        [Test]
        public void Search()
        {
            List<LearningObjectModel> mock = LearningObjectMockData.GetLearningObjectListData();

            _repository.Setup(r => r.Find(It.IsAny<FilterDefinition<LearningObjectModel>>())).Returns(mock);
            _paginationService.Setup(r => r.ApplyPaginationAsync(It.IsAny<IQueryable<LearningObjectModel>>(), It.IsAny<PageRequest>())).Returns(Task.FromResult(mock));
            _service = new LearningObjectService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);

            List<LearningObjectModel> learningObjects = _service.Search("test", new List<string> { "WEB" }, null, It.IsAny<PageRequest>()).Result;
            Assert.AreEqual(learningObjects, mock);
        }
    }
}
