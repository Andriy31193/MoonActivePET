public sealed class ErrorResponse : Response
{
    public ErrorResponse(string message) : base(false, message) {}
}