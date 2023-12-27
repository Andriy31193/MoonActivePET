using Newtonsoft.Json;

public class LoginModel : IModel
{
    [JsonProperty("Username")]
    public string Username { get; set; }

    [JsonProperty("Password")]
    public string Password { get; set; }
}