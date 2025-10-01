using Hangfire.Dashboard;
using System.Text;

namespace Api.Infraestrutura.Hangfire
{
    public class HangfireAuthorizationFilter(string username, string password) : IDashboardAuthorizationFilter
    {
        private readonly string _username = username;
        private readonly string _password = password;

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            string authHeader = httpContext.Request.Headers.Authorization.ToString();

            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                string decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var parts = decodedUsernamePassword.Split(':');
                if (parts.Length == 2)
                {
                    if (parts[0] == _username && parts[1] == _password)
                        return true;
                }
            }
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.WWWAuthenticate = "Basic realm=\"Hangfire Dashboard\"";
            return false;
        }
    }
}
