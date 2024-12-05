using AutoMapper;
using Contents.API.Helpers.Converters;
using Contents.API.Requests.Publisher;
using Contents.API.Requests.Publisher;
using Contents.API.Responses.Publisher;
using Contents.API.Responses.NonSuccessfullResponses;
using Contents.API.Responses.Publisher;
using Contents.Domain.Publisher;
using Contents.Domain.Pagination;
using Contents.Domain.Publisher;
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
    [Route("v{version:apiVersion}/publishers")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiVersion("4.0")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly ILogger<PublisherController> _logger;
        private readonly ILoggerStateFactory _loggerStateFactory;
        private readonly IPublisherService _service;
        private readonly Mapper _mapper_response;
        private readonly Mapper _mapper_request;

        public PublisherController(
            ILogger<PublisherController> logger,
            ILoggerStateFactory loggerStateFactory,
            IPublisherService service,
            DarwinAuthorizationContext authorizationContext
        )
        {
            _loggerStateFactory = loggerStateFactory;
            _logger = logger;
            _service = service;

            _mapper_request = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<PublisherCreateOrUpdateRequest, Publisher>();
                cfg.AllowNullCollections = true;
            }));
            _mapper_response = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<Publisher, PublisherResponse>();

            }));
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ReturnOutput<PublisherResponse>), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> GetAll(
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

                    PageRequest pageRequest = new PageRequest();
                    pageRequest.PageOffset = offset;
                    pageRequest.PageSize = size;

                    List<Publisher> publishers = await _service.GetAll(pageRequest);

                    List<PublisherResponse> response = publishers.ConvertAll(publisher => _mapper_response.Map<PublisherResponse>(publisher));
                    response.ForEach(publisher => publisher._links.Self.Href = Url.Link("GetPublisher", new { publisher.Id }));

                    ReturnOutput<PublisherResponse> formattedPublishers = new()
                    {
                        Items = response,
                        Count = _service.TotalFound
                    };
                    formattedPublishers.MakePaginationLinks(Request, pageRequest.PageOffset, pageRequest.PageSize, _service.TotalFound);

                    return Ok(formattedPublishers);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetAllPublishers - Unhandled Exception");
                    return StatusCode(500);
                }
            }
        }

        [HttpGet("{Id}", Name = "GetPublisher")]
        [Authorize]
        [ProducesResponseType(typeof(PublisherResponse), 200)]
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

                    Publisher publishers = _service.Get(Id);
                    if (publishers == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    PublisherResponse response = _mapper_response.Map<PublisherResponse>(publishers);
                    response._links.Self.Href = Url.Link("GetPublisher", new { response.Id });
                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetPublisher - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(PublisherResponse), 201)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> Create(
            [FromBody] PublisherCreateOrUpdateRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    Publisher publishers = _mapper_request.Map<Publisher>(request);

                    await _service.Create(publishers);

                    PublisherResponse response = _mapper_response.Map<PublisherResponse>(publishers);
                    response._links.Self.Href = Url.Link("GetPublisher", new { response.Id });

                    var routeValues = new { response.Id };
                    return CreatedAtRoute("GetPublisher", routeValues, response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"CreatePublisher - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(typeof(PublisherResponse), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public async Task<IActionResult> Update(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid Id,
            [Required]
            [FromBody] PublisherCreateOrUpdateRequest request)
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

                    Publisher publisher = _mapper_request.Map<Publisher>(request);


                    publisher = await _service.Update(Id, publisher);

                    PublisherResponse response = _mapper_response.Map<PublisherResponse>(publisher);
                    response._links.Self.Href = Url.Link("GetPublisher", new { response.Id });

                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"UpdatePublisher - Unhandled Exception");
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

                    Publisher publisher = _service.Delete(Id);
                    if (publisher == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    return NoContent();
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"DeletePublisher - Unhandled Exception");
                    return StatusCode(500);
                }
        }
    }
}
