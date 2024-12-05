using AutoMapper;
using Contents.API.Requests.LearningObject;
using Contents.API.Requests.Skill;
using Contents.API.Requests.SkillCategory;
using Contents.API.Responses.LearningObject;
using Contents.API.Responses.NonSuccessfullResponses;
using Contents.API.Responses.Skill;
using Contents.Domain.Pagination;
using Contents.Domain.Publisher;
using Contents.Domain.Skill;
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

namespace Contents.API.Controllers
{
    [Route("v{version:apiVersion}/skills-categories")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiVersion("4.0")]
    [ApiController]
    public class SkillCategoryController : ControllerBase
    {
        private readonly ILogger<SkillCategoryController> _logger;
        private readonly ILoggerStateFactory _loggerStateFactory;
        private readonly ISkillCategoryService _service;
        private readonly IPublisherService _publisherService;
        private readonly Mapper _mapper_response;
        private readonly Mapper _mapper_request;

        public SkillCategoryController(
            ILogger<SkillCategoryController> logger,
            ILoggerStateFactory loggerStateFactory,
            ISkillCategoryService service,
            IPublisherService publisherService,
            DarwinAuthorizationContext authorizationContext
        )
        {
            _loggerStateFactory = loggerStateFactory;
            _logger = logger;
            _service = service;
            _publisherService = publisherService;

            _mapper_request = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<SkillCategoryCreateOrUpdateRequest, SkillCategory>();
                cfg.AllowNullCollections = true;
            }));
            _mapper_response = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<SkillCategory, SkillCategoryResponse>();

            }));
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ReturnOutput<SkillCategoryResponse>), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> GetAll(
            [FromQuery] SkillCategoryFilter request
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


                    List<SkillCategory> skillCategories = await _service.GetAll(request);

                    List<SkillCategoryResponse> response = skillCategories.ConvertAll(skillCategory => _mapper_response.Map<SkillCategoryResponse>(skillCategory));
                    response.ForEach(skillCategory => skillCategory._links.Self.Href = Url.Link("GetSkillCategory", new { skillCategory.Id }));

                    ReturnOutput<SkillCategoryResponse> formattedSkillCategorys = new()
                    {
                        Items = response,
                        Count = _service.TotalFound
                    };
                    formattedSkillCategorys.MakePaginationLinks(Request, request.Offset, request.Size, _service.TotalFound);

                    return Ok(formattedSkillCategorys);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetAllSkillCategorys - Unhandled Exception");
                    return StatusCode(500);
                }
            }
        }

        [HttpGet("{Id}", Name = "GetSkillCategory")]
        [Authorize]
        [ProducesResponseType(typeof(SkillCategoryResponse), 200)]
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

                    SkillCategory skillCategories = _service.Get(Id);
                    if (skillCategories == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    SkillCategoryResponse response = _mapper_response.Map<SkillCategoryResponse>(skillCategories);
                    response._links.Self.Href = Url.Link("GetSkillCategory", new { response.Id });
                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetSkillCategory - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(SkillCategoryResponse), 201)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> Create(
            [FromBody] SkillCategoryCreateOrUpdateRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    if (request.PublisherId != Guid.Empty)
                    {
                        Publisher publisher = _publisherService.Get(request.PublisherId);
                        if (publisher == null)
                        {
                            ModelState.AddModelError("publisherId", "Publisher not found");
                            return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                        }
                    }

                    if (request.ParentCategoryId != Guid.Empty)
                    {
                        SkillCategory parentSkillCategory = _service.Get(request.ParentCategoryId);
                        if (parentSkillCategory == null)
                        {
                            ModelState.AddModelError("publisherId", "Publisher not found");
                            return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                        }
                    }

                    SkillCategory skillCategories = _mapper_request.Map<SkillCategory>(request);

                    await _service.Create(skillCategories);

                    SkillCategoryResponse response = _mapper_response.Map<SkillCategoryResponse>(skillCategories);
                    response._links.Self.Href = Url.Link("GetSkillCategory", new { response.Id });

                    var routeValues = new { response.Id };
                    return CreatedAtRoute("GetSkillCategory", routeValues, response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"CreateSkillCategory - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(typeof(SkillCategoryResponse), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public async Task<IActionResult> Update(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid Id,
            [Required]
            [FromBody] SkillCategoryCreateOrUpdateRequest request)
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

                    if (request.PublisherId != Guid.Empty)
                    {
                        Publisher publisher = _publisherService.Get(request.PublisherId);
                        if (publisher == null)
                        {
                            ModelState.AddModelError("publisherId", "Publisher not found");
                            return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                        }
                    }

                    if (request.ParentCategoryId != Guid.Empty)
                    {
                        SkillCategory parentSkillCategory = _service.Get(request.ParentCategoryId);
                        if (parentSkillCategory == null)
                        {
                            ModelState.AddModelError("publisherId", "Publisher not found");
                            return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                        }
                    }
                    

                    SkillCategory skillCategory = _mapper_request.Map<SkillCategory>(request);


                    skillCategory = await _service.Update(Id, skillCategory);

                    SkillCategoryResponse response = _mapper_response.Map<SkillCategoryResponse>(skillCategory);
                    response._links.Self.Href = Url.Link("GetSkillCategory", new { response.Id });

                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"UpdateSkillCategory - Unhandled Exception");
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

                    SkillCategory skillCategory = _service.Delete(Id);
                    if (skillCategory == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    return NoContent();
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"DeleteSkillCategory - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPost("search")]
        [Authorize]
        [ProducesResponseType(typeof(List<SkillCategoryResponse>), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> Search(
            [FromBody] SkillCategorySearchRequest request,
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

                    List<SkillCategory> skillCategories = await _service.Search(request.Query, pageRequest);

                    List<SkillCategoryResponse> response = skillCategories.ConvertAll(skill => _mapper_response.Map<SkillCategoryResponse>(skill));
                    response.ForEach(skillCategory => skillCategory._links.Self.Href = Url.Link("GetSkillCategory", new { skillCategory.Id }));

                    ReturnOutput<SkillCategoryResponse> formattedSkillCategories = new()
                    {
                        Items = response,
                        Count = _service.TotalFound
                    };
                    formattedSkillCategories.MakePaginationLinks(Request, pageRequest.PageOffset, pageRequest.PageSize, _service.TotalFound);

                    return Ok(formattedSkillCategories);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"SkillCategorySearch - Request {request} - Unhandled Exception");
                    return StatusCode(500);
                }
        }
    }
}
