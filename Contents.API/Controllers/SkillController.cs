using AutoMapper;
using Contents.API.Requests.Skill;
using Contents.API.Responses.NonSuccessfullResponses;
using Contents.API.Responses.Skill;
using Contents.Domain.Pagination;
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
    [Route("v{version:apiVersion}/skills")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiVersion("4.0")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly ILogger<SkillController> _logger;
        private readonly ILoggerStateFactory _loggerStateFactory;
        private readonly ISkillService _service;
        private readonly ISkillCategoryService _skillCategoryService;
        private readonly Mapper _mapper_response;
        private readonly Mapper _mapper_request;

        public SkillController(
            ILogger<SkillController> logger,
            ILoggerStateFactory loggerStateFactory,
            ISkillService service,
            ISkillCategoryService skillCategoryService,
            DarwinAuthorizationContext authorizationContext
        )
        {
            _loggerStateFactory = loggerStateFactory;
            _logger = logger;
            _service = service;
            _skillCategoryService = skillCategoryService;

            _mapper_request = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<SkillCreateOrUpdateRequest, Skill>();
                cfg.AllowNullCollections = true;
            }));
            _mapper_response = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<Skill, SkillResponse>();

            }));
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ReturnOutput<SkillResponse>), 200)]
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

                    PageRequest pageRequest = new()
                    {
                        PageOffset = offset,
                        PageSize = size
                    };

                    List<Skill> skills = await _service.GetAll(pageRequest);

                    List<SkillResponse> response = skills.ConvertAll(skill => _mapper_response.Map<SkillResponse>(skill));
                    response.ForEach(skill => skill._links.Self.Href = Url.Link("GetSkill", new { skill.Id }));

                    ReturnOutput<SkillResponse> formattedSkills = new()
                    {
                        Items = response,
                        Count = _service.TotalFound
                    };
                    formattedSkills.MakePaginationLinks(Request, pageRequest.PageOffset, pageRequest.PageSize, _service.TotalFound);

                    return Ok(formattedSkills);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetAllSkills - Unhandled Exception");
                    return StatusCode(500);
                }
            }
        }

        [HttpGet("{Id}", Name = "GetSkill")]
        [Authorize]
        [ProducesResponseType(typeof(SkillResponse), 200)]
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

                    Skill skills = _service.Get(Id);
                    if (skills == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    SkillResponse response = _mapper_response.Map<SkillResponse>(skills);
                    response._links.Self.Href = Url.Link("GetSkill", new { response.Id });
                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"GetSkill - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(SkillResponse), 201)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> Create(
            [FromBody] SkillCreateOrUpdateRequest request)
        {
            using (_logger.BeginScope(_loggerStateFactory.Create(Request.Headers["Transaction-ID"])))
                try
                {
                    
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                    }

                    if (request.CategoryId != Guid.Empty)
                    {
                        SkillCategory skillCategory = _skillCategoryService.Get(request.CategoryId);
                        if (skillCategory == null)
                        {
                            ModelState.AddModelError("categoryId", "Skill Category not found");
                            return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                        }
                    }

                    Skill skills = _mapper_request.Map<Skill>(request);

                    await _service.Create(skills);

                    SkillResponse response = _mapper_response.Map<SkillResponse>(skills);
                    response._links.Self.Href = Url.Link("GetSkill", new { response.Id });

                    var routeValues = new { response.Id };
                    return CreatedAtRoute("GetSkill", routeValues, response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"CreateSkill - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(typeof(SkillResponse), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        [ProducesResponseType(typeof(ResourceNotFoundMessage), 404)]
        public async Task<IActionResult> Update(
            [Required]
            [CustomIdFormatValidation]
            [FromRoute] Guid Id,
            [Required]
            [FromBody] SkillCreateOrUpdateRequest request)
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

                    if (request.CategoryId != Guid.Empty)
                    {
                        SkillCategory skillCategory = _skillCategoryService.Get(request.CategoryId);
                        if (skillCategory == null)
                        {
                            ModelState.AddModelError("categoryId", "Skill Category not found");
                            return BadRequest(NonSuccessfullRequestMessageFormatter.FormatBadRequestResponse(ModelState));
                        }
                    }

                    Skill skill = _mapper_request.Map<Skill>(request);

                    skill = await _service.Update(Id, skill);

                    SkillResponse response = _mapper_response.Map<SkillResponse>(skill);
                    response._links.Self.Href = Url.Link("GetSkill", new { response.Id });

                    return Ok(response);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"UpdateSkill - Unhandled Exception");
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

                    Skill skill = _service.Delete(Id);
                    if (skill == null)
                    {
                        return NotFound(NonSuccessfullRequestMessageFormatter.FormatResourceNotFoundResponse());
                    }

                    return NoContent();
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"DeleteSkill - Unhandled Exception");
                    return StatusCode(500);
                }
        }

        [HttpPost("search")]
        [Authorize]
        [ProducesResponseType(typeof(List<SkillResponse>), 200)]
        [ProducesResponseType(typeof(BadRequestMessage), 400)]
        public async Task<IActionResult> Search(
            [FromBody] SkillSearchRequest request,
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

                    List<Skill> skills = await _service.Search(request.Query, pageRequest);

                    List<SkillResponse> response = skills.ConvertAll(skill => _mapper_response.Map<SkillResponse>(skill));
                    response.ForEach(skill => skill._links.Self.Href = Url.Link("GetSkill", new { skill.Id }));

                    ReturnOutput<SkillResponse> formattedSkills = new()
                    {
                        Items = response,
                        Count = _service.TotalFound
                    };
                    formattedSkills.MakePaginationLinks(Request, pageRequest.PageOffset, pageRequest.PageSize, _service.TotalFound);

                    return Ok(formattedSkills);
                }
                catch (SystemException exception)
                {
                    _logger.LogError(exception, $"SkillSearch - Request {request} - Unhandled Exception");
                    return StatusCode(500);
                }
        }
    }
}
