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

/// <summary>Manage Students</summary>
[ApiController]
[Route("api/students")]  // lowercase route – RESTful URI naming convention
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;
    private readonly IMapper _mapper;

    public StudentsController(IStudentService service, IMapper mapper)
    {
        _service = service;
        _mapper  = mapper;
    }

    /// <summary>Get all students with search, sort, paging, field selection and expand</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<object>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] StudentQueryParams query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<object>>.Ok(result));
    }

    /// <summary>Get student by ID</summary>
    /// <param name="id">Student ID</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var bm = await _service.GetByIdAsync(id);
        if (bm == null) return NotFound(ApiResponse<object>.Fail("Student not found"));
        return Ok(ApiResponse<StudentResponse>.Ok(_mapper.Map<StudentResponse>(bm)));
    }

    // ── Dưới đây là các endpoint NGOÀI YÊU CẦU LAB1 (LAB chỉ yêu cầu GET) ──────
    // Có thể bỏ comment để dùng khi cần thiết.

    // /// <summary>Create a new student</summary>
    // [HttpPost]
    // [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    // [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    // public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    // {
    //     if (!ModelState.IsValid)
    //         return BadRequest(ApiResponse<object>.Fail("Invalid request", ModelState));
    //
    //     var bm       = _mapper.Map<StudentBM>(request);
    //     var created  = await _service.CreateAsync(bm);
    //     var response = _mapper.Map<StudentResponse>(created);
    //     return CreatedAtAction(nameof(GetById), new { id = response.StudentId },
    //         ApiResponse<StudentResponse>.Ok(response, "Student created successfully"));
    // }

    // /// <summary>Update an existing student</summary>
    // [HttpPut("{id:int}")]
    // [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    // public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request)
    // {
    //     if (!ModelState.IsValid)
    //         return BadRequest(ApiResponse<object>.Fail("Invalid request", ModelState));
    //
    //     var bm      = _mapper.Map<StudentBM>(request);
    //     var updated = await _service.UpdateAsync(id, bm);
    //     if (updated == null) return NotFound(ApiResponse<object>.Fail("Student not found"));
    //     return Ok(ApiResponse<StudentResponse>.Ok(_mapper.Map<StudentResponse>(updated)));
    // }

    // /// <summary>Delete a student</summary>
    // [HttpDelete("{id:int}")]
    // [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    // public async Task<IActionResult> Delete(int id)
    // {
    //     var deleted = await _service.DeleteAsync(id);
    //     if (!deleted) return NotFound(ApiResponse<object>.Fail("Student not found"));
    //     return Ok(ApiResponse<object>.Ok(null!, "Student deleted successfully"));
    // }
}
