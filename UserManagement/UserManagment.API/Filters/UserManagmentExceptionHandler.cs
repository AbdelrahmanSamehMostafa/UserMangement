using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserManagment.Application.Exceptions;

namespace UserManagment.API.Filters;

public class UserManagmentExceptionHandler : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case UserNotFoundException:
                var userNotFound = new Error
                {
                    StatusCode = 404,
                    StatusPhrase = $"{context.Exception.Message}",
                    Timestamp = DateTime.Now
                };
                context.Result = new JsonResult(userNotFound) { StatusCode = 404 };
                break;
            case UserRegistrationFailedException:
                var loginFailed = new Error
                {
                    StatusCode = 400,
                    StatusPhrase = $"{context.Exception.Message}",
                    Timestamp = DateTime.Now
                };
                context.Result = new JsonResult(loginFailed) { StatusCode = 400 };
                break;


            case DataException:
                var repeatedData = new Error
                {
                    StatusCode = 409,
                    StatusPhrase = $"{context.Exception.Message}",
                    Timestamp = DateTime.Now
                };
                context.Result = new JsonResult(repeatedData) { StatusCode = 409 };
                break;

            case LoginFailedException:
                var logindata = new Error
                {
                    StatusCode = 409,
                    StatusPhrase = $"{context.Exception.Message}",
                    Timestamp = DateTime.Now
                };
                context.Result = new JsonResult(logindata) { StatusCode = 409 };
                break;
            default:
                var error = new Error
                {
                    StatusCode = 500,
                    StatusPhrase = $"{context.Exception.Message}",
                    Timestamp = DateTime.Now
                };
                context.Result = new JsonResult(error) { StatusCode = 500};
                break;
        }
    }
}