using System.Net;
using com.gbg.modules.utility.Helpers.DTo.Common;
using Microsoft.AspNetCore.Mvc.ViewFeatures; // Ensure you have this using directive

namespace com.gbg.modules.utility.Helpers.Common.Messages
{
    public class GenericErrorHandling
    {
        public static void HandleApiErrorResponse<T>(BaseResponse<T> response, ITempDataDictionary tempData)
        {
            switch (response.statusCode)
            {
                case (int)HttpStatusCode.Conflict:
                    tempData["ErrorMessage"] = response.message;
                    break;

                case (int)HttpStatusCode.NotFound:
                    tempData["ErrorMessage"] = response.message;
                    break;

                case (int)HttpStatusCode.BadRequest:
                    tempData["ErrorMessage"] = response.message;
                    break;

                default:
                    // Handle other errors
                    tempData["ErrorMessage"] = response.message;
                    break;
            }
        }
    }
}