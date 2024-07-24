using Application.DTOs.Response;
using Application.Extensions;
using Application.Services;
using Domain.Entity.CourseEntity;
using Domain.Entity.VehicleEntity;
using Domain.EntityVM;
using System;
using System.Net.Http.Json;

namespace Application.Services
{
    public class EnrollmentService :IEnrollmentService
    {
        private readonly HttpClientService _httpClientService;
        

        public EnrollmentService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            
        }

        private async Task<HttpClient> PrivateClient() => await _httpClientService.GetPrivateClient();

        private static string CheckResponseStatus(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return $"Sorry, an unknown error occurred.{Environment.NewLine}Error Description:{Environment.NewLine}Status Code: {response.StatusCode}{Environment.NewLine}Reason Phrase: {response.ReasonPhrase}";
            return null;
        }

        private static GeneralResponse ErrorOperation(string message) => new(false, message);

        public async Task<IEnumerable<CourseVM>> GetCoursesByStudentAsync(int studentId)
        {
            var client = await PrivateClient();
            var response = await client.GetAsync($"{Constant.GetCoursesByStudent}/{studentId}");

            if (!string.IsNullOrEmpty(CheckResponseStatus(response)))
                return null;

            return await response.Content.ReadFromJsonAsync<IEnumerable<CourseVM>>();
        }

        public async Task<IEnumerable<CourseVM>> GetStudentsByCourseAsync(int courseId)
        {
            var client = await PrivateClient();
            var response = await client.GetAsync($"{Constant.GetStudentsByCourse}/{courseId}");

            if (!string.IsNullOrEmpty(CheckResponseStatus(response)))
                return null;

            return await response.Content.ReadFromJsonAsync<IEnumerable<CourseVM>>();
        }

        public async Task<GeneralResponse> EnrollStudentAsync(string studentId, int courseId, string action)
        {

            var client = await PrivateClient();
            var enrollmentData = new { StudentId = studentId, CourseId = courseId, Action = action };
            var response = await client.PostAsJsonAsync(Constant.EnrollStudent, enrollmentData);

            if (!string.IsNullOrEmpty(CheckResponseStatus(response)))
                return ErrorOperation(CheckResponseStatus(response));

            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
