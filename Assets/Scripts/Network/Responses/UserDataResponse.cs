using Newtonsoft.Json;

public class UserDataResponse : Response
{
    [JsonProperty("UserData")]
    public UserData UserData { get; set; }
}