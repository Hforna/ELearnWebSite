
using System.Net;

namespace User.Api.Excpetions
{
    public class RequestException : BaseProjectException
    {
        public List<string> Errors { get; set; } = [];
        public RequestException(List<string> errors) => Errors = errors;
        public RequestException(string error) => Errors.Add(error);

        public override int GetStatusCode() => (int)HttpStatusCode.BadRequest;

        public override IList<string> GetErrorMessage() => [Message];
    }
}
