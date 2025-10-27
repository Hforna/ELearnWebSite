using Microsoft.AspNetCore.Http;
using Progress.Domain.Dtos;
using Progress.Domain.Exceptions;
using Progress.Domain.Rest;
using Progress.Domain.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Progress.Infrastructure.Rest
{
    public class UserRestService : IUserRestService
    {
        private readonly IHttpClientFactory _httpContext;
        private readonly ITokenReceptor _tokenReceptor;

        public UserRestService(IHttpClientFactory httpContext, ITokenReceptor tokenReceptor)
        {
            _httpContext = httpContext;
            _tokenReceptor = tokenReceptor;
        }

        public async Task<UserInfosDto> GetUserInfos()
        {
            var client = _httpContext.CreateClient("user.api");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenReceptor.GetToken());

            var request = await client.GetAsync("api/users/");

            var response = await request.Content.ReadAsStringAsync();

            if(request.IsSuccessStatusCode)
            {
                var deserialize = JsonSerializer.Deserialize<UserInfosDto>(response);

                return deserialize;
            }
            throw new RestException(response, request.StatusCode);
        }

        public async Task<bool> IsUserLogged()
        {
            var client = _httpContext.CreateClient("user.api");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenReceptor.GetToken());

            var response = await client.GetAsync("api/login/is-user-logged");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var isLogged = JsonSerializer.Deserialize<bool>(result);

                return isLogged;
            }
            return false;
        }
    }
}
