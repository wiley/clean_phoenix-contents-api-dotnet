using AutoMapper;
using Contents.API.Helpers.Converters;
using Contents.API.Requests.TrainingProgram;
using Contents.API.Responses.NonSuccessfullResponses;
using Contents.API.Responses.TrainingProgram;
using Contents.Domain.Pagination;
using Contents.Domain.TrainingProgram;
using Contents.Domain.Utils.CustomValidations;
using Contents.Services.Interfaces;
using DarwinAuthorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WLS.Log.LoggerTransactionPattern;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Contents.API.Controllers
{
    [Route("v{version:apiVersion}/training-programs")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiVersion("4.0")]
    [ApiController]
    public class TrainingProgramController : ControllerBase
    {
        private readonly ILogger<TrainingProgramController> _logger;
        private readonly ILoggerStateFactory _loggerStateFactory;
        private readonly ITrainingProgramService _service;
        private readonly ITrainingProgramStepService _trainingProgramStepService;
        private readonly Mapper _mapper_response;
        private readonly Mapper _mapper_request;

        public TrainingProgramController(
            ILogger<TrainingProgramController> logger,
            ILoggerStateFactory loggerStateFactory,
            ITrainingProgramService service,
            ITrainingProgramStepService trainingProgramStepService,
            DarwinAuthorizationContext authorizationContext
        )
        {
            _loggerStateFactory = loggerStateFactory;
            _logger = logger;
            _service = service;
            _trainingProgramStepService = trainingProgramStepService;

            _mapper_request = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<TrainingProgramCreateRequest, TrainingProgram>();
                cfg.CreateMap<TrainingProgramUpdateRequest, TrainingProgram>();
                cfg.AllowNullCollections = true;
            }));
            _mapper_response = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<TrainingProgram, TrainingProgramResponse>();
                cfg.CreateMap<TrainingProgramStep, TrainingProgramStepSimplifiedResponse>().ConvertUsing<TrainingProgramStepToTrainingProgramStepSimplifiedResponse>();
            }));
        }

        [HttpPut("generate-kafka-events")]
        [Authorize]
        [ProducesResponseType(201)]
        public IActionResult GenerateKafkaEvents()
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
            {
                try
                {
                    _ = _service.GenerateKafkaEvents();

                    return Accepted();
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GenerateKafkaEvents - Unhandled Exception");
                    return StatusCode(500);
                }
            }
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ReturnOutput<TrainingProgramResponse>), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> GetAll(
            [Required]
            [FromQuery] int organizationId,
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
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    PageRequest pageRequest = new()
                    {
                        PageOffset = offset,
                        PageSize = size
                    };

                    List<TrainingProgram> trainingPrograms;

                    trainingPrograms = await _service.SearchByOrganizationId(pageRequest, organizationId);

                    List<TrainingProgramResponse> response = trainingPrograms.ConvertAll(
                        trainingProgram => _mapper_response.Map<TrainingProgramResponse>(trainingProgram)
                    );

                    response.ForEach(trainingProgram => {
                        trainingProgram._links.Self.Href = Url.Link("GetTrainingProgram", new { trainingProgram.Id });
                        trainingProgram.Steps.ForEach(step => {
                            step._links.Self.Href = Url.Link("GetTrainingProgramStep", new { 
                                trainingProgramId = trainingProgram.Id,
                                step.Id 
                            });
                        });
                    });

                    ReturnOutput<TrainingProgramResponse> formattedTrainingProgram = new()
                    {
                        Items = response,
                        Count = _service.TotalFound
                    };

                    List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("organizationId", organizationId.ToString())
                    };

                    formattedTrainingProgram.MakePaginationLinks(Request, pageRequest.PageOffset, pageRequest.PageSize, _service.TotalFound, queryParams);

                    return Ok(formattedTrainingProgram);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetAllTrainingProgram - Unhandled Exception");
                    return StatusCode(500);
                }
            }
        }

        [HttpGet("{Id}", Name = "GetTrainingProgram")]
        [Authorize]
        [ProducesResponseType(typeof(TrainingProgramResponse), 200)]
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

                    TrainingProgram trainingProgram = _service.Get(Id);
                    if (trainingProgram == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    TrainingProgramResponse response = _mapper_response.Map<TrainingProgramResponse>(trainingProgram);
                    response._links.Self.Href = Url.Link("GetTrainingProgram", new { Id = response.Id.ToString() });
                    response.Steps.ForEach(step => {
                        step._links.Self.Href = Url.Link("GetTrainingProgramStep", new
                        {
                            trainingProgramId = trainingProgram.Id,
                            step.Id
                        });
                    });
                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetTrainingProgram - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(TrainingProgramResponse), 201)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> Create(
            [FromBody] TrainingProgramCreateRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    TrainingProgram trainingProgram = _mapper_request.Map<TrainingProgram>(request);

                    await _service.Create(trainingProgram);

                    TrainingProgramResponse response = _mapper_response.Map<TrainingProgramResponse>(trainingProgram);
                    response._links.Self.Href = Url.Link("GetTrainingProgram", new { response.Id });

                    var routeValues = new { response.Id };
                    return CreatedAtRoute("GetTrainingProgram", routeValues, response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"CreateTrainingProgram - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(typeof(TrainingProgramResponse), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public async Task<IActionResult> Update(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid Id,
            [Required]
            [FromBody] TrainingProgramUpdateRequest request)
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

                    TrainingProgramConverters converters = new(_trainingProgramStepService);
                    TrainingProgram trainingProgram = converters.ConvertUpdate(Id, request);

                    trainingProgram = await _service.Update(Id, trainingProgram);

                    TrainingProgramResponse response = _mapper_response.Map<TrainingProgramResponse>(trainingProgram);
                    response._links.Self.Href = Url.Link("GetTrainingProgram", new { response.Id });
                    response.Steps.ForEach(step => {
                        step._links.Self.Href = Url.Link("GetTrainingProgramStep", new
                        {
                            trainingProgramId = trainingProgram.Id,
                            step.Id
                        });
                    });

                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"UpdateTrainingProgram - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpDelete("{Id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public async Task<IActionResult> Delete(
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

                    TrainingProgram trainingProgram = await _service.Delete(Id);
                    if (trainingProgram == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    return NoContent();
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"DeleteTrainingProgram - Unhandled Exception");
                    return StatusCode(500);
                }
        }
    }
}
