using AutoMapper;
using Contents.API.Helpers.Converters;
using Contents.API.Requests.LearningObject;
using Contents.API.Responses.LearningObject;
using Contents.API.Responses.NonSuccessfullResponses;
using Contents.Domain.LearningObject;
using Contents.Domain.Pagination;
using Contents.Domain.Utils.CustomValidations;
using Contents.Services.Interfaces;
using DarwinAuthorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WLS.Log.LoggerTransactionPattern;

namespace Contents.API.Controllers
{
    [Route("v{version:apiVersion}/learning-objects")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiVersion("4.0")]
    [ApiController]
    public class LearningObjectController : ControllerBase
    {
        private readonly ILogger<LearningObjectController> _logger;
        private readonly ILoggerStateFactory _loggerStateFactory;
        private readonly ILearningObjectService _service;
        private readonly Mapper _mapper_response;
        private readonly Mapper _mapper_request;

        public LearningObjectController(
            ILogger<LearningObjectController> logger,
            ILoggerStateFactory loggerStateFactory,
            ILearningObjectService service,
            DarwinAuthorizationContext authorizationContext
        )
        {
            _loggerStateFactory = loggerStateFactory;
            _logger = logger;
            _service = service;

            _mapper_request = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<LearningObjectCreateRequest, LearningObject>();
                cfg.CreateMap<LearningObjectUpdateRequest, LearningObject>();
                cfg.CreateMap<JObject, BsonDocument>().ConvertUsing<JObjectToBsonDocumentConverter>();
                cfg.AllowNullCollections = true;
            }));
            _mapper_response = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<LearningObject, LearningObjectResponse>();
                cfg.CreateMap<BsonDocument, JObject>().ConvertUsing<BsonDocumentToJObjectConverter>();

            }));
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ReturnOutput<LearningObjectResponse>), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> GetAll(
            [FromQuery] LearningObjectFilter request
        )
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    List<LearningObject> learningObjects = await _service.GetAll(request);

                    List<LearningObjectResponse> response = learningObjects.ConvertAll(learningObject => _mapper_response.Map<LearningObjectResponse>(learningObject));
                    response.ForEach(learningObject => learningObject._links.Self.Href = Url.Link("GetLearningObject", new { Id = learningObject.Id.ToString() }));

                    ReturnOutput<LearningObjectResponse> formattedLearningObject = new ReturnOutput<LearningObjectResponse>
                    {
                        Items = response,
                        Count = _service.TotalFound
                    };
                    formattedLearningObject.MakePaginationLinks(Request, request.Offset, request.Size, _service.TotalFound);

                    return Ok(formattedLearningObject);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetAllLearningObject - Unhandled Exception");
                    return StatusCode(500);
                }
            }
        }

        [HttpGet("{Id}", Name = "GetLearningObject")]
        [Authorize]
        [ProducesResponseType(typeof(LearningObjectResponse), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public ActionResult Get(
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

                    LearningObject learningObject = _service.Get(Id);
                    if (learningObject == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    LearningObjectResponse response = _mapper_response.Map<LearningObjectResponse>(learningObject);
                    response._links.Self.Href = Url.Link("GetLearningObject", new { Id = response.Id.ToString() });
                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetLearningObject - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(LearningObjectResponse), 201)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> Create(
            [FromBody] LearningObjectCreateRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    LearningObject learningObject = _mapper_request.Map<LearningObject>(request);

                    await _service.Create(learningObject);

                    LearningObjectResponse response = _mapper_response.Map<LearningObjectResponse>(learningObject);
                    response._links.Self.Href = Url.Link("GetLearningObject", new { response.Id });

                    var routeValues = new { response.Id };
                    return CreatedAtRoute("GetLearningObject", routeValues, response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"CreateLearningObject - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(typeof(LearningObjectResponse), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public async Task<IActionResult> Update(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid Id,
            [Required]
            [FromBody] LearningObjectUpdateRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    if (_service.Get(Id) == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    LearningObject learningObject = _mapper_request.Map<LearningObject>(request);


                    learningObject = await _service.Update(Id, learningObject);

                    LearningObjectResponse response = _mapper_response.Map<LearningObjectResponse>(learningObject);
                    response._links.Self.Href = Url.Link("GetLearningObject", new { response.Id });

                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"UpdateLearningObject - Unhandled Exception");
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

                    LearningObject learningObject = _service.Delete(Id);
                    if (learningObject == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    return NoContent();
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"DeleteLearningObject - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPost("search")]
        [Authorize]
        [ProducesResponseType(typeof(List<LearningObjectResponse>), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> Search(
            [FromBody] LearningObjectSearchRequest request,
            [Required]
            [FromQuery] int organizationId,
            [Range(0, int.MaxValue)]
            [FromQuery] int offset = 0,
            [Range(1, 50)]
            [FromQuery] int size = 20
        )
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    PageRequest pageRequest = new()
                    {
                        PageOffset = offset,
                        PageSize = size
                    };
                    
                    List<LearningObject> learningObjects = await _service.Search(request.Query, request.Types, organizationId, request.Metadata, pageRequest);

                    List<LearningObjectResponse> response = learningObjects.ConvertAll(learningObject => _mapper_response.Map<LearningObjectResponse>(learningObject));
                    response.ForEach(learningObject => learningObject._links.Self.Href = Url.Link("GetLearningObject", new { Id = learningObject.Id.ToString() }));

                    ReturnOutput<LearningObjectResponse> formattedLearningObjects = new()
                    {
                        Items = response,
                        Count = _service.TotalFound
                    };
                    formattedLearningObjects.MakePaginationLinks(Request, pageRequest.PageOffset, pageRequest.PageSize, _service.TotalFound);

                    return Ok(formattedLearningObjects);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"LearningObjectSearch - Request {request} - Unhandled Exception");
                    return StatusCode(500);
                }
        }
    }
}
