using Microsoft.AspNetCore.Http;
using Payment.Domain.DTOs;
using Payment.Domain.Exceptions;
using Payment.Domain.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services.Rest
{
    public class CourseRestService : ICourseRestService
    {
        private readonly IHttpClientFactory _httpClient;

        public CourseRestService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CourseDto?> GetCourse(string courseId)
        {
            var client = _httpClient.CreateClient("course.api");
            var response = await client.GetAsync($"api/course/{courseId}");

            if(response.IsSuccessStatusCode)
            {
                var responseMessage = await response.Content.ReadAsStreamAsync();

                var deserialize = JsonSerializer.Deserialize<CourseDto>(responseMessage);

                return deserialize;
            } else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new RestException(ResourceExceptMessages.COURSE_NOT_FOUND, System.Net.HttpStatusCode.NotFound);
            }
            throw new Exception("Internal server error");
        }
    }
}
