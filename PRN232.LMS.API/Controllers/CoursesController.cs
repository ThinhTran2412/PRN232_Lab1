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

/// <summary>Manage Courses</summary>
[ApiController]
[Route("api/courses")]  // lowercase route – RESTful URI naming convention
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;
    private readonly IMapper _mapper;

    public CoursesController(ICourseService service, IMapper mapper)
    {
        _service = service;
        _mapper  = mapper;
    }

    /// <summary>Get all courses with search, sort, paging, field selection and expand</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<object>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CourseQueryParams query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<object>>.Ok(result));
    }

    /// <summary>Get course by ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var bm = await _service.GetByIdAsync(id);
        if (bm == null) return NotFound(ApiResponse<object>.Fail("Course not found"));
        return Ok(ApiResponse<CourseResponse>.Ok(_mapper.Map<CourseResponse>(bm)));
    }

    // ── Dưới đây là các endpoint NGOÀI YÊU CẦU LAB1 (LAB chỉ yêu cầu GET) ──────
    // Có thể bỏ comment để dùng khi cần thiết.

    // /// <summary>Create a new course</summary>
    // [HttpPost]
    // [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    // [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    // public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    // {
    //     if (!ModelState.IsValid)
    //         return BadRequest(ApiResponse<object>.Fail("Invalid request", ModelState));
    //
    //     var bm       = _mapper.Map<CourseBM>(request);
    //     var created  = await _service.CreateAsync(bm);
    //     var response = _mapper.Map<CourseResponse>(created);
    //     return CreatedAtAction(nameof(GetById), new { id = response.CourseId },
    //         ApiResponse<CourseResponse>.Ok(response, "Course created successfully"));
    // }

    // /// <summary>Update an existing course</summary>
    // [HttpPut("{id:int}")]
    // [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    // public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest request)
    // {
    //     if (!ModelState.IsValid)
    //         return BadRequest(ApiResponse<object>.Fail("Invalid request", ModelState));
    //
    //     var bm      = _mapper.Map<CourseBM>(request);
    //     var updated = await _service.UpdateAsync(id, bm);
    //     if (updated == null) return NotFound(ApiResponse<object>.Fail("Course not found"));
    //     return Ok(ApiResponse<CourseResponse>.Ok(_mapper.Map<CourseResponse>(updated)));
    // }

    // /// <summary>Delete a course</summary>
    // [HttpDelete("{id:int}")]
    // [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    // public async Task<IActionResult> Delete(int id)
    // {
    //     var deleted = await _service.DeleteAsync(id);
    //     if (!deleted) return NotFound(ApiResponse<object>.Fail("Course not found"));
    //     return Ok(ApiResponse<object>.Ok(null!, "Course deleted successfully"));
    // }
}
