using Microsoft.AspNetCore.Http;
using Payment.Domain.DTOs;
using Payment.Domain.Exceptions;
using Payment.Domain.Services.Rest;
using Payment.Domain.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services.Rest
{
    public class UserRestService : IUserRestService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ITokenReceptor _tokenReceptor;
        private readonly string? _token;

        public UserRestService(ITokenReceptor tokenReceptor, IHttpClientFactory httpClient)
        {
            _tokenReceptor = tokenReceptor;
            _httpClient = httpClient;
            _token = _tokenReceptor.GetToken()!;
        }

        public async Task<UserInfoDto> GetUserInfos()
        {
            var client = _httpClient.CreateClient("user.api");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await client.GetAsync("api/users/");

            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var deserialize = JsonSerializer.Deserialize<UserInfoDto>(content);

                return deserialize;
            }
            throw new RestException(await response.Content.ReadAsStringAsync(), response.StatusCode);
        }

        public async Task<ProfileDto> GetUserProfile(string userId)
        {
            var client = _httpClient.CreateClient("user.api");

            var request = await client.GetAsync($"api/profiles/{userId}");

            var response = await request.Content.ReadAsStringAsync();

            if(request.IsSuccessStatusCode)
            {
                var deserialize = JsonSerializer.Deserialize<ProfileDto>(response);

                return deserialize!;
            }
            throw new RestException(response);
        }
    }
}
