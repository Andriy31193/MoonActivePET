public class ErrorResponse : Response
{
    public ErrorResponse(string errorMsg)
    {
        this.Success = false;
        this.Message = errorMsg;
    }
}