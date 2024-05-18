public sealed class DefaultResponse : Response
{
    public DefaultResponse(bool success, string message) : base(success, message) {}
}