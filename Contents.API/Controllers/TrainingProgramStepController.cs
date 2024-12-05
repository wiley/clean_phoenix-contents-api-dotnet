using AutoMapper;
using Contents.API.Helpers.Converters;
using Contents.API.Requests.TrainingProgram;
using Contents.API.Responses.LearningObject;
using Contents.API.Responses.NonSuccessfullResponses;
using Contents.API.Responses.TrainingProgram;
using Contents.Domain.LearningObject;
using Contents.Domain.Pagination;
using Contents.Domain.TrainingProgram;
using Contents.Domain.Utils.CustomValidations;
using Contents.Services.Interfaces;
using DarwinAuthorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WLS.Log.LoggerTransactionPattern;

namespace Contents.API.Controllers
{
    [Route("v{version:apiVersion}/training-programs/{trainingProgramId}/steps")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiVersion("4.0")]
    [ApiController]
    public class TrainingProgramStepController : ControllerBase
    {
        private readonly ILogger<TrainingProgramStepController> _logger;
        private readonly ILoggerStateFactory _loggerStateFactory;
        private readonly ITrainingProgramStepService _service;
        private readonly ITrainingProgramService _trainingProgramService;
        private readonly ILearningObjectService _learningObjectService;
        private readonly Mapper _mapper_response;
        private readonly Mapper _mapper_request;

        public TrainingProgramStepController(
            ILogger<TrainingProgramStepController> logger,
            ILoggerStateFactory loggerStateFactory,
            ITrainingProgramStepService service,
            ITrainingProgramService trainingProgramService,
            ILearningObjectService learningObjectService,
            DarwinAuthorizationContext authorizationContext
        )
        {
            _loggerStateFactory = loggerStateFactory;
            _logger = logger;
            _service = service;
            _trainingProgramService = trainingProgramService;
            _learningObjectService = learningObjectService;

            _mapper_request = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<TrainingProgramStepCreateOrUpdateRequest, TrainingProgramStep>();
                cfg.AllowNullCollections = true;
            }));
            _mapper_response = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<TrainingProgramStep, TrainingProgramStepResponse>();
                cfg.CreateMap<LearningObjectStep, LearningObjectStepResponse>();
                cfg.CreateMap<LearningObject, LearningObjectResponse>();
                cfg.CreateMap<BsonDocument, JObject>().ConvertUsing<BsonDocumentToJObjectConverter>();
            }));
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ReturnOutput<TrainingProgramStepResponse>), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> GetAll(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid trainingProgramId,
            [Range(0, int.MaxValue)]
            [FromQuery] int offset = 0,
            [Range(1, 50)]
            [FromQuery] int size = 20
        )
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                try
                {
                    TrainingProgram trainingProgram = _trainingProgramService.Get(trainingProgramId);
                    if (trainingProgram == null)
                    {
                        ModelState.AddModelError("trainingProgramId", "Training Program Not Found");
                    }

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    PageRequest pageRequest = new()
                    {
                        PageOffset = offset,
                        PageSize = size
                    };

                    List<TrainingProgramStep> steps = await _service.GetAll(trainingProgramId, pageRequest);

                    List<TrainingProgramStepResponse> response = steps.ConvertAll(trainingProgram => _mapper_response.Map<TrainingProgramStepResponse>(trainingProgram));
                    response.ForEach(step => {
                        step._links.Self.Href = Url.Link("GetTrainingProgramStep", new { trainingProgramId, step.Id });
                        step.LearningObjects.ForEach(learningObject =>
                        {
                            learningObject.LearningObject._links.Self.Href = Url.Link("GetLearningObject", new { learningObject.LearningObject.Id });
                        });
                    });

                    ReturnOutput<TrainingProgramStepResponse> formattedTrainingProgramSteps = new()
                    {
                        Items = response,
                        Count = _service.TotalFound
                    };
                    formattedTrainingProgramSteps.MakePaginationLinks(Request, pageRequest.PageOffset, pageRequest.PageSize, _service.TotalFound);

                    return Ok(formattedTrainingProgramSteps);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetAllTrainingProgramStep - TrainingProgram: {trainingProgramId} - Unhandled Exception");
                    return StatusCode(500);
                }
            }
        }

        [HttpGet("{Id}", Name = "GetTrainingProgramStep")]
        [Authorize]
        [ProducesResponseType(typeof(TrainingProgramStepResponse), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public ActionResult Get(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid trainingProgramId,
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid Id
        )
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    TrainingProgramStep step = _service.Get(trainingProgramId, Id);
                    if (step == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }
                    
                    TrainingProgramStepResponse response = _mapper_response.Map<TrainingProgramStepResponse>(step);
                    response._links.Self.Href = Url.Link("GetTrainingProgramStep", new { trainingProgramId, response.Id });
                    response.LearningObjects.ForEach(learningObject =>
                    {
                        learningObject.LearningObject._links.Self.Href = Url.Link("GetLearningObject", new { learningObject.LearningObject.Id });
                    });
                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetTrainingProgramStep - TrainingProgram: {trainingProgramId} - Step: {Id} - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(TrainingProgramStepResponse), 201)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> Create(
            [CustomIdFormatValidation]
            [Required]
            [FromRoute] Guid trainingProgramId,
            [FromBody] TrainingProgramStepCreateOrUpdateRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    TrainingProgram trainingProgram = _trainingProgramService.Get(trainingProgramId);
                    if (trainingProgram == null)
                    {
                        ModelState.AddModelError("trainingProgramId", "Training Program Not Found");
                    }

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    TrainingProgramStepConverters converters = new(_learningObjectService);
                    TrainingProgramStep trainingProgramStep = converters.Convert(request);

                    await _service.Create(trainingProgramId, trainingProgramStep);

                    TrainingProgramStepResponse response = _mapper_response.Map<TrainingProgramStepResponse>(trainingProgramStep);
                    response._links.Self.Href = Url.Link("GetTrainingProgramStep", new { trainingProgramId, response.Id });
                    response.LearningObjects.ForEach(learningObject =>
                    {
                        learningObject.LearningObject._links.Self.Href = Url.Link("GetLearningObject", new { learningObject.LearningObject.Id });
                    });

                    var routeValues = new { trainingProgramId, response.Id };
                    return CreatedAtRoute("GetTrainingProgramStep", routeValues, response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"CreateTrainingProgramStep - TrainingProgram: {trainingProgramId} - Request: {request} - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(typeof(TrainingProgramStepResponse), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public async Task<IActionResult> Update(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid trainingProgramId,
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid Id,
            [Required]
            [FromBody] TrainingProgramStepCreateOrUpdateRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    TrainingProgramStep step = _service.Get(trainingProgramId, Id);
                    if (step == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    TrainingProgramStepConverters converters = new(_learningObjectService);
                    TrainingProgramStep trainingProgramStep = converters.Convert(request);
                    trainingProgramStep.Id = Id;

                    trainingProgramStep = await _service.Update(trainingProgramId, trainingProgramStep);

                    TrainingProgramStepResponse response = _mapper_response.Map<TrainingProgramStepResponse>(trainingProgramStep);
                    response._links.Self.Href = Url.Link("GetTrainingProgramStep", new { trainingProgramId, response.Id });
                    response.LearningObjects.ForEach(learningObject =>
                    {
                        learningObject.LearningObject._links.Self.Href = Url.Link("GetLearningObject", new { learningObject.LearningObject.Id });
                    });

                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"UpdateTrainingProgramStep - TrainingProgram: {trainingProgramId} - Step: {Id} - Request: {request} - Unhandled Exception");
                    return StatusCode(500);
                }
        }
        
        [HttpDelete("{Id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public ActionResult Delete(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid trainingProgramId,
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid Id
       )
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    TrainingProgramStep step = _service.Get(trainingProgramId, Id);
                    if (step == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    _service.Delete(trainingProgramId, Id);

                    return NoContent();
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"DeleteTrainingProgramStep - TrainingProgram: {trainingProgramId} - Step: {Id} - Unhandled Exception");
                    return StatusCode(500);
                }
        }
    }
}
