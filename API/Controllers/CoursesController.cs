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
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-course")]
        public async Task<ActionResult<List<CourseVM>>> GetCourses()
        {
            var courses = await _context.Courses
                .Select(c => new CourseVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();

            return Ok(courses);
        }

        [HttpPost("create-course")]
        public async Task<ActionResult<CourseVM>> CreateCourse(CourseVM courseVM)
        {
            if (courseVM == null)
            {
                return BadRequest();
            }

            var course = new Course
            {
                Name = courseVM.Name,
                Description = courseVM.Description
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            courseVM.Id = course.Id; // Set the ID to the newly created course's ID

            return CreatedAtAction(nameof(GetCourses), new { id = course.Id }, courseVM);
        }

        [HttpPut("edit-course/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, CourseVM courseVM)
        {
            if (id != courseVM.Id)
            {
                return BadRequest();
            }

            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            course.Name = courseVM.Name;
            course.Description = courseVM.Description;

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("delete-course/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
      
        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
