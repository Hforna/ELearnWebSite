using Progress.Domain.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Progress.Domain.Token;
using System.Net;
using Progress.Domain.Exceptions;
using Progress.Domain.Dtos;

namespace Progress.Infrastructure.Rest
{
    public class CourseRestService : ICourseRestService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ITokenReceptor _tokenReceptor;

        public CourseRestService(IHttpClientFactory httpClient, ITokenReceptor tokenReceptor)
        {
            _httpClient = httpClient;
            _tokenReceptor = tokenReceptor;
        }

        public async Task<int> CountCourseLessons(string courseId)
        {
            var client = _httpClient.CreateClient("course.api");

            var request = await client.GetAsync($"api/courses/{courseId}/count-lessons");

            var response = await request.Content.ReadAsStringAsync();

            if(request.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<int>(response);
            }
            throw new RestException(response, request.StatusCode);
        }

        public async Task<QuizDto> GetQuiz(string quizId, string courseId)
        {
            var client = _httpClient.CreateClient("course.api");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenReceptor.GetToken());

            var request = await client.GetAsync($"api/quizzes/{courseId}/{quizId}");
            var response = await request.Content.ReadAsStringAsync();

            if(request.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<QuizDto>(response)!;
            }
            throw new RestException(response, request.StatusCode);
        }

        public async Task<LessonInfosDto> LessonInfos(long lessonId)
        {
            var client = _httpClient.CreateClient("course.api");

            var request = await client.GetAsync($"api/lessons/{lessonId}/infos");

            var response = await request.Content.ReadAsStringAsync();

            if(request.IsSuccessStatusCode)
            {
                var deserialize = JsonSerializer.Deserialize<LessonInfosDto>(response, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            throw new RestException(response, request.StatusCode);
        }

        public async Task<bool> UserGotCourse(string courseId)
        {
            var client = _httpClient.CreateClient("course.api");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenReceptor.GetToken());

            var jsonContent = JsonSerializer.Serialize(new { courseId = courseId, });
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var request = await client.PostAsync("api/courses/user-got-course", stringContent);

            var response = await request.Content.ReadAsStringAsync();

            if(request.IsSuccessStatusCode)
            {
                var deserialize = JsonSerializer.Deserialize<bool>(response);

                return deserialize;
            }
            throw new RestException(response, request.StatusCode);
        }


    }
}
