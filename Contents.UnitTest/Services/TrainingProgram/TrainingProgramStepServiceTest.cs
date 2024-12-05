using Contents.Domain.Pagination;
using Contents.Domain.TrainingProgram;
using TraningProgramModel = Contents.Domain.TrainingProgram.TrainingProgram;
using Contents.Infrastructure.Interface.Mongo;
using Contents.Services;
using Contents.Services.Interfaces;
using Contents.UnitTest.MockData;
using DarwinAuthorization.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contents.UnitTest.Services.TrainingProgram
{
    public class TrainingProgramStepServiceTest
    {
        private ITrainingProgramStepService _service;
        private readonly Mock<ITrainingProgramService> _trainingProgramService;
        private readonly Mock<IMongoRepository<TraningProgramModel>> _repository;
        private readonly Mock<IPaginationService<TrainingProgramStep>> _paginationService;
        private readonly Mock<ILogger<TrainingProgramStepService>> _logger;
        private readonly Mock<DarwinAuthorizationContext> _authorizationContext;
        private TraningProgramModel _trainingProgram;

        public TrainingProgramStepServiceTest()
        {
            _repository = new Mock<IMongoRepository<TraningProgramModel>>();
            _logger = new Mock<ILogger<TrainingProgramStepService>>();
            _paginationService = new Mock<IPaginationService<TrainingProgramStep>>();
            _trainingProgramService = new Mock<ITrainingProgramService>();
            _authorizationContext = new Mock<DarwinAuthorizationContext>();
        }

        [SetUp]
        public void Setup()
        {
            _trainingProgram = TrainingProgramMockData.GetTrainingProgram(Guid.NewGuid());
            _trainingProgramService.Setup(r => r.Get(It.IsAny<Guid>())).Returns(_trainingProgram);
        }

        [Test]
        public async Task GetAll_Expect_Steps()
        {
            List<TrainingProgramStep> mock = TrainingProgramMockData.GetStepListData();
            _trainingProgram.Steps.AddRange(mock);

            Mock<PageRequest> _pageRequest = new Mock<PageRequest>();

            _paginationService.Setup(r => r.ApplyPaginationAsync(It.IsAny<IQueryable<TrainingProgramStep>>(), _pageRequest.Object)).Returns(Task.FromResult(mock));
            _service = new TrainingProgramStepService(_repository.Object, _logger.Object, _paginationService.Object, _trainingProgramService.Object, _authorizationContext.Object);

            List<TrainingProgramStep> steps = await _service.GetAll(_trainingProgram.Id, _pageRequest.Object);
            CollectionAssert.AreEqual(_trainingProgram.Steps, mock);
        }

        [Test]
        public void Get_Step_Succeed()
        {
            TrainingProgramStep mock = TrainingProgramMockData.GetStep(Guid.NewGuid());
            _trainingProgram.Steps.Add(mock);

            _repository.Setup(r => r.FindOne(It.IsAny<FilterDefinition<TraningProgramModel>>())).Returns(_trainingProgram);
            _service = new TrainingProgramStepService(_repository.Object, _logger.Object, _paginationService.Object, _trainingProgramService.Object, _authorizationContext.Object);

            TrainingProgramStep step = _service.Get(_trainingProgram.Id, mock.Id);
            Assert.AreEqual(step, mock);
        }

        [Test]
        public void Get_Expect_Null()
        {
            TrainingProgramStep mock = TrainingProgramMockData.GetStep(Guid.NewGuid());
            TraningProgramModel nullTrainingProgram = null;

            _repository.Setup(r => r.FindOne(It.IsAny<FilterDefinition<TraningProgramModel>>())).Returns(nullTrainingProgram);
            _service = new TrainingProgramStepService(_repository.Object, _logger.Object, _paginationService.Object, _trainingProgramService.Object, _authorizationContext.Object);

            TrainingProgramStep step = _service.Get(_trainingProgram.Id, mock.Id);
            Assert.IsNull(step);
        }


        [Test]
        public async Task Create_Succeed()
        {
            TrainingProgramStep mock = TrainingProgramMockData.GetStep(Guid.NewGuid());

            _repository.Setup(r => r.InsertOneAsync(It.IsAny<TraningProgramModel>())).Returns(Task.FromResult(mock));
            _service = new TrainingProgramStepService(_repository.Object, _logger.Object, _paginationService.Object, _trainingProgramService.Object, _authorizationContext.Object);

            TrainingProgramStep step = await _service.Create(_trainingProgram.Id, mock);
            Assert.AreEqual(step.Id, mock.Id);
            Assert.AreEqual(step.Title, mock.Title);
            Assert.AreEqual(step.Description, mock.Description);
            Assert.AreEqual(_trainingProgram.Steps.First().Id, mock.Id);
        }


        [Test]
        public async Task Update_Succeed()
        {
            TrainingProgramStep mock = TrainingProgramMockData.GetStep(Guid.NewGuid());
            _trainingProgram.Steps.Add(mock);

            TrainingProgramStep updatedStep = mock;
            updatedStep.Title = "Updated";
            updatedStep.Description = "Updated";

            _trainingProgramService.Setup(r => r.Get(It.IsAny<Guid>())).Returns(_trainingProgram);
            _trainingProgramService.Setup(r => r.Update(It.IsAny<Guid>(), It.IsAny<TraningProgramModel>())).Returns(Task.FromResult(_trainingProgram));
            _repository.Setup(r => r.FindOne(It.IsAny<FilterDefinition<TraningProgramModel>>())).Returns(_trainingProgram);

            _service = new TrainingProgramStepService(_repository.Object, _logger.Object, _paginationService.Object, _trainingProgramService.Object, _authorizationContext.Object);

            updatedStep = await _service.Update(_trainingProgram.Id, updatedStep);
            Assert.True(mock.Title == updatedStep.Title);
            Assert.True(mock.Description == updatedStep.Description);
            Assert.GreaterOrEqual(updatedStep.UpdatedAt, mock.UpdatedAt);
        }


        [Test]
        public void Delete_Succeed()
        {
            TrainingProgramStep mock = TrainingProgramMockData.GetStep(Guid.NewGuid());
            _trainingProgram.Steps.Add(mock);

            _trainingProgramService.Setup(r => r.Get(It.IsAny<Guid>())).Returns(_trainingProgram);
            _trainingProgramService.Setup(r => r.Update(It.IsAny<Guid>(), It.IsAny<TraningProgramModel>())).Returns(Task.FromResult(_trainingProgram));

            _service = new TrainingProgramStepService(_repository.Object, _logger.Object, _paginationService.Object, _trainingProgramService.Object, _authorizationContext.Object);
            _service.Delete(_trainingProgram.Id, mock.Id);
            _trainingProgram.Steps.RemoveAll(step => step.Id == mock.Id);
            Assert.IsTrue(_trainingProgram.Steps.Count == 0);
        }
    }
}
