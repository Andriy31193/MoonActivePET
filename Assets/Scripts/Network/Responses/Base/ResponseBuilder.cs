public static class ResponseBuilder
{
    public static ErrorResponse BuildErrorResponse(string message) => new ErrorResponse(message);
}