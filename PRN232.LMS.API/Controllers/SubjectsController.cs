using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Models.Common;
using PRN232.LMS.API.Models.Requests;
using PRN232.LMS.API.Models.Responses;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.QueryParams;
using PRN232.LMS.Services.Shared;

namespace PRN232.LMS.API.Controllers;

/// <summary>Manage Subjects</summary>
[ApiController]
[Route("api/subjects")]  // lowercase route – RESTful URI naming convention
[Produces("application/json")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _service;
    private readonly IMapper _mapper;

    public SubjectsController(ISubjectService service, IMapper mapper)
    {
        _service = service;
        _mapper  = mapper;
    }

    /// <summary>Get all subjects with search, sort and paging</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<object>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] SubjectQueryParams query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<object>>.Ok(result));
    }

    /// <summary>Get subject by ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var bm = await _service.GetByIdAsync(id);
        if (bm == null) return NotFound(ApiResponse<object>.Fail("Subject not found"));
        return Ok(ApiResponse<SubjectResponse>.Ok(_mapper.Map<SubjectResponse>(bm)));
    }

    // ── Dưới đây là các endpoint NGOÀI YÊU CẦU LAB1 (LAB chỉ yêu cầu GET) ──────
    // Có thể bỏ comment để dùng khi cần thiết.

    /// <summary>Create a new subject</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSubjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request", ModelState));
    
        var bm       = _mapper.Map<SubjectBM>(request);
        var created  = await _service.CreateAsync(bm);
        var response = _mapper.Map<SubjectResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.SubjectId },
            ApiResponse<SubjectResponse>.Ok(response, "Subject created successfully"));
    }

    /// <summary>Update an existing subject</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSubjectRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request", ModelState));
    
        var bm      = _mapper.Map<SubjectBM>(request);
        var updated = await _service.UpdateAsync(id, bm);
        if (updated == null) return NotFound(ApiResponse<object>.Fail("Subject not found"));
        return Ok(ApiResponse<SubjectResponse>.Ok(_mapper.Map<SubjectResponse>(updated)));
    }

    /// <summary>Delete a subject</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Subject not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Subject deleted successfully"));
    }
}
