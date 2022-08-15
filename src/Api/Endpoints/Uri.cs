namespace Onion.Api.Endpoints;

public static class Uri
{
    public static class Identity
    {
        public const string Tag = nameof(Identity);
        public const string Login = "/api/identity/login";
        public const string Refresh = "/api/identity/refresh";
        public const string Register = "/api/identity/register";
        public const string GetUsers = "/api/users/get";
    }
}
