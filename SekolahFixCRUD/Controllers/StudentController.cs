using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SekolahFixCRUD.DTOs.Student;
using SekolahFixCRUD.Interfaces;
using System.Security.Claims;

namespace SekolahFixCRUD.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentController> _logger;

    public StudentController(IStudentService studentService, ILogger<StudentController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var result = await _studentService.GetProfileAsync(userId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(StudentUpdateDto updateDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        _logger.LogInformation("Student {UserId} updating profile", userId);
        var result = await _studentService.UpdateProfileAsync(userId, updateDto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("courses")]
    public async Task<IActionResult> GetCourses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _studentService.GetAvailableCoursesAsync(pageNumber, pageSize);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> Enroll(StudentEnrollmentDto enrollmentDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        _logger.LogInformation("Student {UserId} enrolling in courses", userId);
        var result = await _studentService.EnrollCoursesAsync(userId, enrollmentDto);
        return StatusCode((int)result.StatusCode, result);
    }
}
