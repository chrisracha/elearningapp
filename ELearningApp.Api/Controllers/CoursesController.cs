using Microsoft.AspNetCore.Mvc;
using ELearningApp.Api.Services;
using ELearningApp.Api.Models.Entities;

namespace ELearningApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    
    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        return Ok(courses);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Course>> GetCourse(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null) return NotFound();
        return Ok(course);
    }
    
    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<Course>>> GetFeaturedCourses()
    {
        var courses = await _courseService.GetFeaturedCoursesAsync();
        return Ok(courses);
    }
    
    [HttpGet("popular")]
    public async Task<ActionResult<IEnumerable<Course>>> GetPopularCourses()
    {
        // For now, return featured courses as popular courses
        var courses = await _courseService.GetFeaturedCoursesAsync();
        return Ok(courses);
    }
    
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<Course>>> GetCoursesByCategory(int categoryId)
    {
        var courses = await _courseService.GetCoursesByCategoryAsync(categoryId);
        return Ok(courses);
    }
    
    [HttpGet("instructor/{instructorId}")]
    public async Task<ActionResult<IEnumerable<Course>>> GetCoursesByInstructor(string instructorId)
    {
        var courses = await _courseService.GetCoursesByInstructorAsync(instructorId);
        return Ok(courses);
    }
}