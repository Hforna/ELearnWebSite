using Course.Domain.DTOs;
using Course.Domain.Services.Rest;
using Course.Domain.Services.Token;
using Course.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Course.Infrastructure.Services.Rest
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ITokenReceptor _tokenReceptor;
        private readonly string _token;

        public UserService(IHttpClientFactory httpClient, ITokenReceptor tokenReceptor)
        {
            _tokenReceptor = tokenReceptor;
            _httpClient = httpClient;
            _token = _tokenReceptor.GetToken();
        }

        public UserService(IHttpClientFactory httpClient) => _httpClient = httpClient;

        public async Task<UserInfosDto> GetUserInfos()
        {
            var client = _httpClient.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await client.GetAsync("https://user.api:8081/api/user/user-infos");

            if(response.IsSuccessStatusCode)
            {
                var userResponse = await response.Content.ReadAsStringAsync();

                var userInfosFormat = JsonSerializer.Deserialize<UserInfosDto>(userResponse);

                return userInfosFormat!;
            }
            throw new RestException(response.Content.ToString());
            
        }

        public async Task<List<string>> GetUserRoles(Guid uid)
        {
            var client = _httpClient.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await client.GetAsync($"https://user.api:8081/api/user/get-user-roles/{uid.ToString()}");

            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var roles = JsonSerializer.Deserialize<List<String>>(result);

                return roles!;
            }
            throw new RestException($"Error for user uid, status code: {response.StatusCode}");
        }
    }
}
