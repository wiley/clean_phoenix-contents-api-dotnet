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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WLS.Log.LoggerTransactionPattern;

namespace Contents.API.Controllers
{
    [Route("v{version:apiVersion}/training-programs/{trainingProgramId}/steps/{stepId}/learning-objects")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiVersion("4.0")]
    [ApiController]
    public class TrainingProgramStepLearningObjectController : ControllerBase
    {
        private readonly ILogger<TrainingProgramStepLearningObjectController> _logger;
        private readonly ILoggerStateFactory _loggerStateFactory;
        private readonly ITrainingProgramStepLearningObjectService _service;
        private readonly ITrainingProgramStepService _trainingProgramStepService;
        private readonly ILearningObjectService _learningObjectService;
        private readonly Mapper _mapper_response;

        public TrainingProgramStepLearningObjectController(
            ILogger<TrainingProgramStepLearningObjectController> logger,
            ILoggerStateFactory loggerStateFactory,
            ITrainingProgramStepLearningObjectService service,
            ITrainingProgramStepService trainingProgramStepService,
            ILearningObjectService learningObjectService,
            DarwinAuthorizationContext authorizationContext
        )
        {
            _loggerStateFactory = loggerStateFactory;
            _logger = logger;
            _service = service;
            _trainingProgramStepService = trainingProgramStepService;
            _learningObjectService = learningObjectService;

            _mapper_response = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<LearningObjectStep, LearningObjectStepResponse>();
                cfg.CreateMap<LearningObject, LearningObjectResponse>();
                cfg.CreateMap<BsonDocument, JObject>().ConvertUsing<BsonDocumentToJObjectConverter>();
            }));
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ReturnOutput<LearningObjectStepResponse>), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> GetAll(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid trainingProgramId,
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid stepId,
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
                    TrainingProgramStep step = _trainingProgramStepService.Get(trainingProgramId, stepId);
                    if (step == null)
                    {
                        ModelState.AddModelError("stepId", "Training Program Step Not Found");
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

                    List<LearningObjectStep> learningObjects = await _service.GetAll(trainingProgramId, stepId, pageRequest);

                    List<LearningObjectStepResponse> response = learningObjects.ConvertAll(learningObjects => _mapper_response.Map<LearningObjectStepResponse>(learningObjects));
                    response.ForEach(learningObject => {
                            learningObject.LearningObject._links.Self.Href = Url.Link("GetLearningObject", new { learningObject.LearningObject.Id });
                    });

                    ReturnOutput<LearningObjectStepResponse> formattedLearningObjectStepResponse = new()
                    {
                        Items = response,
                        Count = _service.TotalFound
                    };
                    formattedLearningObjectStepResponse.MakePaginationLinks(Request, pageRequest.PageOffset, pageRequest.PageSize, _service.TotalFound);

                    return Ok(formattedLearningObjectStepResponse);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetAllTrainingProgramStepLearningObjects - TrainingProgram: {trainingProgramId} - TrainingProgramStep: {stepId} - Unhandled Exception");
                    return StatusCode(500);
                }
            }
        }

        [HttpGet("{Id}", Name = "GetTrainingProgramStepLearningObject")]
        [Authorize]
        [ProducesResponseType(typeof(LearningObjectStepResponse), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public ActionResult Get(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid trainingProgramId,
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid stepId,
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

                    LearningObjectStep learningObject = _service.Get(trainingProgramId, stepId, Id);
                    if (learningObject == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }
                    
                    LearningObjectStepResponse response = _mapper_response.Map<LearningObjectStepResponse>(learningObject);
                    response.LearningObject._links.Self.Href = Url.Link("GetLearningObject", new { learningObject.LearningObject.Id });
                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetTrainingProgramStepLO - TrainingProgram: {trainingProgramId} - Step: {stepId} - LO: {Id} - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(LearningObjectStepResponse), 201)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> Create(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid trainingProgramId,
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid stepId,
            [FromBody] TrainingProgramStepLOCreateRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    TrainingProgramStep step = _trainingProgramStepService.Get(trainingProgramId, stepId);
                    if (step == null)
                    {
                        ModelState.AddModelError("stepId", "Training Program Step Not Found");
                    }
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    LearningObject learningObject = null;
                    if (request.LearningObject != null)
                    {
                        learningObject = _learningObjectService.Get(request.LearningObject.Id);
                        if (learningObject == null)
                        {
                            ModelState.AddModelError("learningObject.id", "Learning Object Not Found");
                        }
                    }

                    LearningObjectStep learningObjectStep = new()
                    {
                        LearningObject = learningObject,
                        IsMandatory = request.IsMandatory
                    };

                    await _service.Create(trainingProgramId, stepId, learningObjectStep);

                    LearningObjectStepResponse response = _mapper_response.Map<LearningObjectStepResponse>(learningObjectStep);
                    response.LearningObject._links.Self.Href = Url.Link("GetLearningObject", new { response.LearningObject.Id });

                    var routeValues = new { trainingProgramId, stepId, response.LearningObject.Id };
                    return CreatedAtRoute("GetTrainingProgramStepLearningObject", routeValues, response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"CreateTrainingProgramStepLearningObject - TrainingProgram: {trainingProgramId} - Step: {stepId} - LO: {request} - Unhandled Exception");
                    return StatusCode(500);
                }
                catch (JsonReaderException exception)
                {
                    ModelState.AddModelError("IsMandatory", "The IsMandatory is in wrong format.");
                    _logger.LogError(exception, $"CreateTrainingProgramStepLearningObject - TrainingProgram: {trainingProgramId} - Step: {stepId} - LO: {request} - The IsMandatory is in wrong format.");
                    return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                }
        }
        
        [HttpPatch("{Id}")]
        [Authorize]
        [ProducesResponseType(typeof(LearningObjectStepResponse), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public async Task<IActionResult> Update(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid trainingProgramId,
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid stepId,
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid Id,
            [Required]
            [FromBody] TrainingProgramStepLOUpdateRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    LearningObjectStep learningObject = _service.Get(trainingProgramId, stepId, Id);
                    if (learningObject == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }
                    learningObject.IsMandatory = request.IsMandatory;

                    learningObject = await _service.Update(trainingProgramId, stepId, learningObject);

                    LearningObjectStepResponse response = _mapper_response.Map<LearningObjectStepResponse>(learningObject);
                    response.LearningObject._links.Self.Href = Url.Link("GetLearningObject", new { response.LearningObject.Id });
                    
                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"UpdateTrainingProgramStepLearningObject - TrainingProgram: {trainingProgramId} - Step: {stepId} - LO: {Id} - Unhandled Exception");
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
            [FromRoute] Guid stepId,
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

                    LearningObjectStep learningObject = _service.Get(trainingProgramId, stepId, Id);
                    if (learningObject == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    _service.Delete(trainingProgramId, stepId, Id);

                    return NoContent();
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"DeleteTrainingProgramStepLearningObject - TrainingProgram: {trainingProgramId} - Step: {stepId} - LO: {Id} - Unhandled Exception");
                    return StatusCode(500);
                }
        }
    }
}
