using MassTransit.SagaStateMachine;
using Newtonsoft.Json.Linq;
using Sqids;

namespace Course.Api.Middlewares
{
    public class IdValidtorMiddleware : IMiddleware
    {
        private readonly IServiceProvider _serviceProvider;

        public IdValidtorMiddleware(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            var routes = context.Request.RouteValues;
            var query = context.Request.Query;

            try
            {
                var scope = _serviceProvider.CreateScope();
                var sqids = scope.ServiceProvider.GetRequiredService<SqidsEncoder<long>>();

                if (routes.Count > 0)
                {
                    var dict = routes.ToDictionary(d => d.Key, f => f.Value.ToString());
                    ValidateKeyWords(dict, sqids);
                }

                if (query.Count > 0)
                {
                    var dict = query.ToDictionary(d => d.Key, d => d.Value.ToString());
                    ValidateKeyWords(dict, sqids);
                }

                if (!string.IsNullOrEmpty(body))
                {
                    var json = JObject.Parse(body);
                    var listId = new Dictionary<string, string>();
                    if (json.TryGetValue("quizId", StringComparison.CurrentCultureIgnoreCase, out var value))
                    {
                        listId.Add("quizId", value.ToString());
                    }
                    else if (json.TryGetValue("moduleId", StringComparison.CurrentCultureIgnoreCase, out var moduleId))
                    {
                        listId.Add("moduleId", moduleId.ToString());
                    }
                    else if (json.TryGetValue("courseId", StringComparison.CurrentCultureIgnoreCase, out var courseId))
                    {
                        listId.Add("courseId", courseId.ToString());
                    }

                    if (listId.Count > 0)
                    {
                        ValidateWordAsSqids(listId, sqids);
                    }
                }
            }
            catch (Exception rex)
            {
                throw new Exception(rex.Message);
            }

            await next(context);
        }

        void ValidateKeyWords(Dictionary<string, string> dict, SqidsEncoder<long> sqids)
        {
            var listId = new Dictionary<string, string>();

            if (dict.TryGetValue("quizId", out var value))
            {
                listId.Add("quizId", value.ToString());
            }
            else if (dict.TryGetValue("moduleId", out var moduleId))
            {
                listId.Add("moduleId", moduleId.ToString());
            }
            else if (dict.TryGetValue("courseId", out var courseId))
            {
                listId.Add("courseId", courseId.ToString());
            }

            if (listId.Count > 0)
            {
                ValidateWordAsSqids(listId, sqids);
            }
        }

        void ValidateWordAsSqids(Dictionary<string, string> values, SqidsEncoder<long> sqids)
        {
            foreach (var id in values)
            {
                var decodeId = sqids.Decode(id.Value).SingleOrDefault(defaultValue: 0);

                if (decodeId == 0)
                    throw new Exception($"The id fied {id} is in invalid format");
            }
        }
    }
}
