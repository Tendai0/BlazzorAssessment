using Application.Contracts;
using Domain.Entity.CourseEntity;
using Domain.EntityVM;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepo _courseRepository;

        public CoursesController(ICourseRepo courseRepository)
        {
            _courseRepository = courseRepository;
        }

        [HttpGet("get-course/{UserId}")]
        public async Task<ActionResult<List<CourseVM>>> GetCourses(string UserId)
        {
            var courses = await _courseRepository.GetCoursesAsync(UserId);
            return Ok(courses);
        }
        [HttpGet("registered-courses/{userId}")]
        public async Task<ActionResult<List<CourseVM>>> GetRegisteredCourses(string userId)
        {
            var courses = await _courseRepository.GetRegisteredCoursesAsync(userId);
            return Ok(courses);
        }

        [HttpPost("create-course")]
        public async Task<ActionResult<CourseVM>> CreateCourse(CourseVM courseVM)
        {
            if (courseVM == null)
            {
                return BadRequest();
            }

            var createdCourse = await _courseRepository.CreateCourseAsync(courseVM);
            return CreatedAtAction(nameof(GetCourses), new { id = createdCourse.Id }, createdCourse);
        }

        [HttpPut("edit-course/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, CourseVM courseVM)
        {
            if (id != courseVM.Id)
            {
                return BadRequest();
            }

            try
            {
                await _courseRepository.UpdateCourseAsync(courseVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("delete-course/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                await _courseRepository.DeleteCourseAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
