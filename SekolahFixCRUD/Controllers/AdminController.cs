using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SekolahFixCRUD.DTOs.Course;
using SekolahFixCRUD.DTOs.Teacher;
using SekolahFixCRUD.Interfaces;
using System.Security.Claims;

namespace SekolahFixCRUD.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IAdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    // --- Course Management ---
    [HttpGet("courses")]
    public async Task<IActionResult> GetCourses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _adminService.GetAllCoursesAsync(pageNumber, pageSize);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("course")]
    public async Task<IActionResult> CreateCourse(CourseCreateDto createDto)
    {
        var result = await _adminService.CreateCourseAsync(createDto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("course/{id}")]
    public async Task<IActionResult> UpdateCourse(int id, CourseUpdateDto updateDto)
    {
        var result = await _adminService.UpdateCourseAsync(id, updateDto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("course/{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var result = await _adminService.DeleteCourseAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("course/restore/{id}")]
    public async Task<IActionResult> RestoreCourse(int id)
    {
        var result = await _adminService.RestoreCourseAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    // --- Teacher Management ---
    [HttpGet("teachers")]
    public async Task<IActionResult> GetTeachers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _adminService.GetAllTeachersAsync(pageNumber, pageSize);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("teacher")]
    public async Task<IActionResult> CreateTeacher(TeacherCreateDto createDto)
    {
        var result = await _adminService.CreateTeacherAsync(createDto);
        return StatusCode((int)result.StatusCode, result);
    }

    // --- Student Workflow ---
    [HttpGet("students")]
    public async Task<IActionResult> GetStudents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _adminService.GetAllStudentsAsync(pageNumber, pageSize);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("student/approve/{id}")]
    public async Task<IActionResult> ApproveStudent(int id)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Admin {AdminId} approving student {StudentId}", adminId, id);
        var result = await _adminService.ApproveStudentAsync(id, adminId!);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("student/reject/{id}")]
    public async Task<IActionResult> RejectStudent(int id)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Admin {AdminId} rejecting student {StudentId}", adminId, id);
        var result = await _adminService.RejectStudentAsync(id, adminId!);
        return StatusCode((int)result.StatusCode, result);
    }
}
