using Domain.EntityVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface ICourseRepo
    {
        Task<List<CourseVM>> GetRegisteredCoursesAsync(string userId);
        Task<List<CourseVM>> GetCoursesAsync(string userId);
        Task<CourseVM> CreateCourseAsync(CourseVM courseVM);
        Task UpdateCourseAsync(CourseVM courseVM);
        Task DeleteCourseAsync(int id);
    }
}
