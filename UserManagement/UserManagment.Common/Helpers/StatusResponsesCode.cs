namespace UserManagment.Common.Helpers
{
    public static class StatusResponsesCode
    {
        // Status Codes for HTTP Responses
        public const int StatusCodeOK = 200; // Success
        public const int StatusCodeBadRequest = 400; // Bad Request
        public const int StatusCodeUnauthorized = 401; // Unauthorized
        public const int StatusCodeForbidden = 403; // Forbidden
        public const int StatusCodeNotFound = 404; // Not Found
        public const int StatusCodeConflict = 409; // Conflict
        public const int StatusCodeInternalServerError = 500; // Internal Server Error
    }
}