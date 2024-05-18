using Newtonsoft.Json;

public abstract class Response
{
    [JsonProperty("Success")]
    public bool Success { get; }
    [JsonProperty("Message")]
    public string Message { get; }

    public Response(bool success, string msg) 
    {
        this.Success = success;
        this.Message = msg;
    }
}