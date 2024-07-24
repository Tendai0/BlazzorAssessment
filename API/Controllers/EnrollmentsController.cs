using Application.DTOs.Response;
using Domain.Entity.CourseEntity;
using Domain.EntityVM;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnrollmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("courses-by-student/{studentId}")]
        public async Task<ActionResult<List<CourseVM>>> GetCoursesByStudent(string studentId)
        {
            var courses = await _context.Enrollments
                .Where(e => e.UserId == studentId)
                
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("students-by-course/{courseId}")]
        public async Task<ActionResult<List<Student>>> GetStudentsByCourse(int courseId)
        {
            var students = await _context.Enrollments
                .Where(e => e.CourseId == courseId)             
                .ToListAsync();

            return Ok(students);
        }

        [HttpPost("enroll")]
        public async Task<ActionResult> EnrollStudent(string studentId, int courseId, string action)
        {
            if (action == "register")
            {
                // Register the student
                var enrollment = new Enrollment
                {
                    UserId = studentId,
                    CourseId = courseId
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                return Ok();
            }
            else if (action == "unregister")
            {
                // Unregister the student
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.UserId == studentId && e.CourseId == courseId);

                if (enrollment == null)
                {
                    return NotFound();
                }

                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        } 
    }
}
