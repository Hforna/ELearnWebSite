using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Progress.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Progress.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using Progress.Domain.Exceptions;

namespace IntegrationTests
{
    public class ConfigureApplication : WebApplicationFactory<Program>
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(service =>
            {
                service.RemoveAll(typeof(ProjectDbContext));
                service.AddDbContext<ProjectDbContext>(opts =>
                {
                    opts.UseInMemoryDatabase("TestDatabase");
                });
            });
            base.ConfigureWebHost(builder);
        }

        public async Task<HttpClient> GetClientWithToken()
        {
            var client = this.CreateClient();

            var request = await client.PostAsJsonAsync("user.api/api/login", new { email = "usertest@gmail.com", password = "userpassword" });
            var response = await request.Content.ReadAsStringAsync();

            if(request.IsSuccessStatusCode)
            {
                var deserialize = JsonSerializer.Deserialize<TokenDto>(response);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserialize!.accessToken);

                return client;
            } else
            {
                throw new RestException(response, request.StatusCode);
            }
        }

        private sealed record TokenDto()
        {
            public string? accessToken { get; set; }
            public string? refreshToken { get; set; }
        }

    }
}
