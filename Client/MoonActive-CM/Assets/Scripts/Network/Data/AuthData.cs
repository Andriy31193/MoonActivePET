using Network.Enums;
using Newtonsoft.Json;

namespace Network.Request.Data
{
    public sealed class AuthData : RequestData
    {
        public override Endpoint NetworkEndpoint => Endpoint.Login;

        [JsonProperty("Username")]
        public string Username { get; }

        [JsonProperty("Password")]
        public string Password { get; }


        public AuthData(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
    }
}