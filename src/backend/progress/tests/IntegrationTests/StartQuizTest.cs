using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class StartQuizTest : IClassFixture<ConfigureApplication>
    {
        private readonly ConfigureApplication _app;

        public StartQuizTest(ConfigureApplication app) => _app = app;

        [Fact]
        public async Task User_Doesnt_Got_Course_Error()
        {
            var sqids = new SqidsEncoder<long>(new SqidsOptions()
            {
                Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
                MinLength = 4
            });

            var client = await _app.GetClientWithToken();
            var request = await client.GetAsync($"api/attempt/course/{sqids.Encode(4)}/quiz/{sqids.Encode(5)}/start");

            var response = request.Content.ReadAsStreamAsync();

            Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);
        }
    }
}
