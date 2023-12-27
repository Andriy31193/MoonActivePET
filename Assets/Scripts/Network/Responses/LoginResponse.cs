using System;
using Newtonsoft.Json;

[Serializable]
public class LoginResponse : Response
{
    [JsonProperty("Token")]
    public string Token { get; set; }

}