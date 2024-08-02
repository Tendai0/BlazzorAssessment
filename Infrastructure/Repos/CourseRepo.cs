using Application.Contracts;
using Domain.Entity.CourseEntity;
using Domain.EntityVM;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class CourseRepo: ICourseRepo
    {
        private readonly AppDbContext _context;

        public CourseRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CourseVM>> GetCoursesAsync(string userId)
        {
            return await _context.Courses
                .Include(c => c.Enrollments)
                .Select(c => new CourseVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsRegistered = c.Enrollments.Any(e => e.UserId == userId)
                })
                .ToListAsync();
        }
        public async Task<List<CourseVM>> GetRegisteredCoursesAsync(string userId)
        {
            var courses = await _context.Courses
                .Where(c => c.Enrollments.Any(e => e.UserId == userId))
                .Select(c => new CourseVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsRegistered = true
                })
                .ToListAsync();

            return courses;
        }

        public async Task<CourseVM> CreateCourseAsync(CourseVM courseVM)
        {
            if (courseVM == null)
            {
                throw new ArgumentNullException(nameof(courseVM));
            }

            var course = new Course
            {
                Name = courseVM.Name,
                Description = courseVM.Description
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            courseVM.Id = course.Id;
            return courseVM;
        }

        public async Task UpdateCourseAsync(CourseVM courseVM)
        {
            if (courseVM == null)
            {
                throw new ArgumentNullException(nameof(courseVM));
            }

            var course = await _context.Courses.FindAsync(courseVM.Id);

            if (course == null)
            {
                throw new KeyNotFoundException("Course not found");
            }

            course.Name = courseVM.Name;
            course.Description = courseVM.Description;

            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                throw new KeyNotFoundException("Course not found");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}
