namespace com.gbg.modules.utility.Extentions
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the token is present in session
            var token = context.Session.GetString("AuthToken");

            // Allow access to the login page and the access denied page
            if (context.Request.Path.StartsWithSegments("/Authentication/Login") ||
              context.Request.Path.StartsWithSegments("/AccessDenied") ||
                context.Request.Path.StartsWithSegments("/") ||
                context.Request.Path.StartsWithSegments("/Authentication/ForgetPassword"))
            {
                await _next(context);
                return;
            }

            if (string.IsNullOrEmpty(token))
            {
                // Redirect to the access denied page if the token is missing
                context.Response.Redirect("/AccessDenied");
                return;
            }

            // Continue processing the request
            await _next(context);
        }
    }
}