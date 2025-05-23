﻿using Course.Domain.DTOs;
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
            _token = _tokenReceptor.GetToken()!;
        }

        public async Task<UserInfosDto?> GetUserInfos()
        {
            var client = _httpClient.CreateClient("user.api");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await client.GetAsync("api/user/user-infos");

            if(response.IsSuccessStatusCode)
            {
                var userResponse = await response.Content.ReadAsStringAsync();

                var userInfosFormat = JsonSerializer.Deserialize<UserInfosDto>(userResponse);

                return userInfosFormat!;
            }
            throw new RestException(await response.Content.ReadAsStringAsync());    
        }

        public async Task<UserInfosDto?> GetUserInfosById(string id)
        {
            var client = _httpClient.CreateClient("user.api");

            var response = await client.GetAsync($"api/user/user-infos/{id}");
            var userResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var serializer = JsonSerializer.Deserialize<UserInfosDto>(userResponse);

                return serializer;
            }
            throw new RestException(userResponse);
        }

        public async Task<List<string>?> GetUserRoles(Guid uid)
        {
            if (string.IsNullOrEmpty(_token))
                return null;

            var client = _httpClient.CreateClient("user.api");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await client.GetAsync($"api/user/get-user-roles/{uid.ToString()}");
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {

                var roles = JsonSerializer.Deserialize<List<String>>(result);

                return roles!;
            }
            throw new RestException(result);
        }

        public async Task<bool> IsUserLogged()
        {
            var client = _httpClient.CreateClient("user.api");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
        
            var response = await client.GetAsync("api/login/is-user-logged");

            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var isLogged = JsonSerializer.Deserialize<bool>(result);

                return isLogged;
            }
            return false;
        }
    }
}
