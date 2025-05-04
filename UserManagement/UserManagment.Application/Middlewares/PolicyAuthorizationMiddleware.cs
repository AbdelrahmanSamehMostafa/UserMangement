using Microsoft.AspNetCore.Http;
using System.Text.Json;
using UserManagment.Common.Helpers;

public class PolicyAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public PolicyAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            // Get the `RequiresPolicy` attribute on the endpoint
            var requiredPolicyAttribute = endpoint.Metadata.GetMetadata<RequiresPolicyAttribute>();
            if (requiredPolicyAttribute != null)
            {
                // Retrieve the `PoliciesNames` claim from the JWT
                var userPolicies = context.User.FindFirst("PoliciesNames")?.Value;
                if (userPolicies == null)
                {
                    throw new CustomException(ErrorResponseMessage.Forbidden);
                }

                var userPolicyList = userPolicies.Split(',').Select(p => p.Trim()).ToList();

                // Check if the required policy is in the user's policies
                if (!userPolicyList.Contains(requiredPolicyAttribute.PolicyName))
                {
                    throw new CustomException(ErrorResponseMessage.Forbidden);
                }
            }

            // If the policy check passes, proceed to the next middleware
            await _next(context);
        }
        catch (CustomException ex)
        {
            // Set up a structured response for CustomException
            var response = new BaseResponse<object>
            {
                IsSuccess = false,
                StatusCode = StatusResponsesCode.StatusCodeForbidden, // Use the Forbidden status code
                Message = ex.Message
            };

            context.Response.StatusCode = StatusResponsesCode.StatusCodeForbidden;
            context.Response.ContentType = "application/json";

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
