using Contents.Domain.Pagination;
using TrainingProgramModel = Contents.Domain.TrainingProgram.TrainingProgram;
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
using System.Reflection;


namespace Contents.UnitTest.Services.TrainingProgram
{
    public class TrainingProgramServiceTest
    {
        private ITrainingProgramService _service;
        private readonly Mock<IMongoRepository<TrainingProgramModel>> _repository;
        private readonly Mock<IPaginationService<TrainingProgramModel>> _paginationService;
        private readonly Mock<ILogger<TrainingProgramService>> _logger;
        private readonly Mock<DarwinAuthorizationContext> _authorizationContext;

        public TrainingProgramServiceTest()
        {
            _repository = new Mock<IMongoRepository<TrainingProgramModel>>();
            _logger = new Mock<ILogger<TrainingProgramService>>();
            _paginationService = new Mock<IPaginationService<TrainingProgramModel>>();
            _authorizationContext = new Mock<DarwinAuthorizationContext>();
        }

        [Test]
        public async Task GetAll()
        {
            List<TrainingProgramModel> mock = TrainingProgramMockData.GetTrainingProgramListData();
            Mock<PageRequest> _pageRequest = new Mock<PageRequest>();

            _paginationService.Setup(r => r.ApplyPaginationAsync(It.IsAny<IQueryable<TrainingProgramModel>>(), _pageRequest.Object)).Returns(Task.FromResult(mock));
            _service = new TrainingProgramService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);

            List<TrainingProgramModel> trainingPrograms = await _service.GetAll(_pageRequest.Object);
            CollectionAssert.AreEqual(mock, trainingPrograms);
        }

        [Test]
        public async Task SearchByOrganizationId()
        {
            List<TrainingProgramModel> mock = TrainingProgramMockData.GetTrainingProgramListData();
            Mock<PageRequest> _pageRequest = new Mock<PageRequest>();

            _paginationService.Setup(r => r.ApplyPaginationAsync(It.IsAny<IQueryable<TrainingProgramModel>>(), _pageRequest.Object)).Returns(Task.FromResult(mock));
            _service = new TrainingProgramService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);

            List<TrainingProgramModel> trainingPrograms = await _service.SearchByOrganizationId(_pageRequest.Object, 534);
            CollectionAssert.AreEqual(mock, trainingPrograms);
        }

        [Test]
        public void Get()
        {
            Guid id = Guid.Parse("c3008b5b-0a73-4def-8496-ea850f154d9a");
            TrainingProgramModel mock = TrainingProgramMockData.GetTrainingProgram(id);

            _repository.Setup(r => r.FindById(id)).Returns(mock);
            _service = new TrainingProgramService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);

            TrainingProgramModel trainingProgram = _service.Get(id);
            Assert.AreEqual(trainingProgram, mock);
        }

        [Test]
        public async Task Create()
        {
            Guid id = Guid.Parse("c3008b5b-0a73-4def-8496-ea850f154d9a");
            TrainingProgramModel mock = TrainingProgramMockData.GetTrainingProgram(id);

            _repository.Setup(r => r.InsertOneAsync(It.IsAny<TrainingProgramModel>())).Returns(Task.FromResult(mock));
            _service = new TrainingProgramService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);

            await _service.Create(mock);
            Assert.AreEqual(mock, mock);
        }

        [Test]
        public async Task Update()
        {
            Guid id = Guid.Parse("c3008b5b-0a73-4def-8496-ea850f154d9a");
            TrainingProgramModel mock = TrainingProgramMockData.GetTrainingProgram(id);

            mock.Title = "Updated";
            mock.Description = "Updated";

            TrainingProgramModel updatedTrainingProgram = mock;

            _repository.Setup(r => r.ReplaceOneAsync(It.IsAny<TrainingProgramModel>())).Returns(Task.FromResult(updatedTrainingProgram));
            _service = new TrainingProgramService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);

            await _service.Update(id, updatedTrainingProgram);
            Assert.True(mock.Title == updatedTrainingProgram.Title);
            Assert.True(mock.Description == updatedTrainingProgram.Description);
            Assert.GreaterOrEqual(updatedTrainingProgram.UpdatedAt, mock.UpdatedAt);
        }

        [Test]
        public void Delete()
        {
            Guid id = Guid.Parse("c3008b5b-0a73-4def-8496-ea850f154d9a");
            TrainingProgramModel mock = TrainingProgramMockData.GetTrainingProgram(id);

            _repository.Setup(r => r.DeleteById(It.IsAny<Guid>())).Returns(mock);
            _service = new TrainingProgramService(_repository.Object, _logger.Object, _paginationService.Object, _authorizationContext.Object);

            TrainingProgramModel enrollment = _service.Delete(id);
            Assert.AreEqual(enrollment, mock);
        }
    }
}
