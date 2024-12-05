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
    public class TrainingProgramStepLearningObjectServiceTest
    {
        private ITrainingProgramStepLearningObjectService _service;
        private readonly Mock<IMongoRepository<TraningProgramModel>> _repository;
        private readonly Mock<IPaginationService<LearningObjectStep>> _paginationService;
        private readonly Mock<ILogger<TrainingProgramStepLearningObjectService>> _logger;
        private readonly Mock<ITrainingProgramService> _trainingProgramService;
        private readonly Mock<ITrainingProgramStepService> _stepService;
        private readonly Mock<DarwinAuthorizationContext> _authorizationContext;
        private TraningProgramModel _trainingProgram;
        private TrainingProgramStep _step;

        public TrainingProgramStepLearningObjectServiceTest()
        {
            _repository = new Mock<IMongoRepository<TraningProgramModel>>();
            _logger = new Mock<ILogger<TrainingProgramStepLearningObjectService>>();
            _paginationService = new Mock<IPaginationService<LearningObjectStep>>();
            _trainingProgramService = new Mock<ITrainingProgramService>();
            _stepService = new Mock<ITrainingProgramStepService>();
            _authorizationContext = new Mock<DarwinAuthorizationContext>();
        }

        [SetUp]
        public void Setup()
        {
            _trainingProgram = TrainingProgramMockData.GetTrainingProgram(Guid.NewGuid());
            _step = TrainingProgramMockData.GetStep(Guid.NewGuid());
            _trainingProgram.Steps.Add(_step);
            _trainingProgramService.Setup(r => r.Get(It.IsAny<Guid>())).Returns(_trainingProgram);
        }

        [Test]
        public async Task GetAll_Succeed()
        {
            List<LearningObjectStep> mock = TrainingProgramMockData.GetLearningObjectStepListData();
            _trainingProgram.Steps.First().LearningObjects.AddRange(mock);

            Mock<PageRequest> _pageRequest = new Mock<PageRequest>();
            _stepService.Setup(s => s.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(_step);
            _paginationService.Setup(r => r.ApplyPaginationAsync(It.IsAny<IQueryable<LearningObjectStep>>(), _pageRequest.Object)).Returns(Task.FromResult(mock));
            _service = new TrainingProgramStepLearningObjectService(
                _repository.Object,
                _logger.Object,
                _paginationService.Object,
                _trainingProgramService.Object,
                _stepService.Object,
                _authorizationContext.Object
            );

            List<LearningObjectStep> steps = await _service.GetAll(_trainingProgram.Id, _step.Id, _pageRequest.Object);
            CollectionAssert.AreEqual(_step.LearningObjects, mock);
        }

        
        [Test]
        public void Get_Step_Succeed()
        {
            LearningObjectStep mock = TrainingProgramMockData.GetLearningObjectStep(Guid.NewGuid());
            _trainingProgram.Steps.First().LearningObjects.Add(mock);

            _repository.Setup(r => r.FindOne(It.IsAny<FilterDefinition<TraningProgramModel>>())).Returns(_trainingProgram);
            _service = new TrainingProgramStepLearningObjectService(
                _repository.Object,
                _logger.Object,
                _paginationService.Object,
                _trainingProgramService.Object,
                _stepService.Object,
                _authorizationContext.Object
            );

            LearningObjectStep learningObject = _service.Get(_trainingProgram.Id, _step.Id, mock.LearningObject.Id);
            Assert.AreEqual(learningObject, mock);
        }
        
        [Test]
        public void Get_Expect_Null()
        {
            LearningObjectStep mock = TrainingProgramMockData.GetLearningObjectStep(Guid.NewGuid());
            TraningProgramModel nullTrainingProgram = null;

            _repository.Setup(r => r.FindOne(It.IsAny<FilterDefinition<TraningProgramModel>>())).Returns(nullTrainingProgram);
            _service = new TrainingProgramStepLearningObjectService(
                _repository.Object,
                _logger.Object,
                _paginationService.Object,
                _trainingProgramService.Object,
                _stepService.Object,
                _authorizationContext.Object
            );

            LearningObjectStep learningObject = _service.Get(_trainingProgram.Id, _step.Id, mock.LearningObject.Id);
            Assert.IsNull(learningObject);
        }

        
        [Test]
        public async Task Create_Succeed()
        {
            LearningObjectStep mock = TrainingProgramMockData.GetLearningObjectStep(Guid.NewGuid());

            _stepService.Setup(r => r.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(_step);
            _step.LearningObjects.Add(mock);
            _stepService.Setup(r => r.Update(It.IsAny<Guid>(), It.IsAny<TrainingProgramStep>())).Returns(Task.FromResult(_step));

            _service = new TrainingProgramStepLearningObjectService(
                _repository.Object,
                _logger.Object,
                _paginationService.Object,
                _trainingProgramService.Object,
                _stepService.Object,
                _authorizationContext.Object
            );

            LearningObjectStep learningObject = await _service.Create(_trainingProgram.Id, _step.Id, mock);
            Assert.AreEqual(learningObject.LearningObject.Id, mock.LearningObject.Id);
            Assert.AreEqual(learningObject.IsMandatory, mock.IsMandatory);
            Assert.AreEqual(_trainingProgram.Steps.First().LearningObjects.First().LearningObject.Id, mock.LearningObject.Id);
        }

        
        [Test]
        public async Task Update_Succeed()
        {
            LearningObjectStep mock = TrainingProgramMockData.GetLearningObjectStep(Guid.NewGuid());
            _step.LearningObjects.Add(mock);

            LearningObjectStep updatedLearningObject = mock;
            updatedLearningObject.IsMandatory = false;

            _stepService.Setup(r => r.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(_step);

            int index = _step.LearningObjects.FindIndex(learningObject => learningObject.LearningObject.Id == mock.LearningObject.Id);
            _step.LearningObjects[index] = mock;

            _stepService.Setup(r => r.Update(It.IsAny<Guid>(), It.IsAny<TrainingProgramStep>())).Returns(Task.FromResult(_step));

            _service = new TrainingProgramStepLearningObjectService(
                _repository.Object,
                _logger.Object,
                _paginationService.Object,
                _trainingProgramService.Object,
                _stepService.Object,
                _authorizationContext.Object
            );

            updatedLearningObject = await _service.Update(_trainingProgram.Id, _step.Id, updatedLearningObject);
            Assert.True(mock.IsMandatory == updatedLearningObject.IsMandatory);
        }

        
        [Test]
        public void Delete_Succeed()
        {
            LearningObjectStep mock = TrainingProgramMockData.GetLearningObjectStep(Guid.NewGuid());
            _step.LearningObjects.Add(mock);

            _stepService.Setup(r => r.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(_step);

            _step.LearningObjects.RemoveAll(learningObject => learningObject.LearningObject.Id == mock.LearningObject.Id);

            _stepService.Setup(r => r.Update(It.IsAny<Guid>(), It.IsAny<TrainingProgramStep>())).Returns(Task.FromResult(_step));

            _service = new TrainingProgramStepLearningObjectService(
                _repository.Object,
                _logger.Object,
                _paginationService.Object,
                _trainingProgramService.Object,
                _stepService.Object,
                _authorizationContext.Object
            );
            _service.Delete(_trainingProgram.Id, _step.Id, mock.LearningObject.Id);
            Assert.IsTrue(_step.LearningObjects.Count == 0);
        }
    }
}
