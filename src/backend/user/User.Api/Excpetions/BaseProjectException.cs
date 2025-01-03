namespace User.Api.Excpetions
{
    public abstract class BaseProjectException : SystemException
    {
        public abstract int GetStatusCode();
        public abstract IList<string> GetErrorMessage();
    }
}
