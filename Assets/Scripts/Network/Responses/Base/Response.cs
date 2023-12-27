using Newtonsoft.Json;

public abstract class Response
{
    [JsonProperty("Success")]
    public bool Success { get; set; }
    [JsonProperty("Message")]
    public string Message { get; set; }
}