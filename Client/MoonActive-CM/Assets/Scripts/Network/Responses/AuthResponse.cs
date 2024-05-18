using System;
using Newtonsoft.Json;

namespace Network.Responses
{
    [Serializable]
    public sealed class AuthResponse : Response
    {
        [JsonProperty("Token")]
        public string Token { get; }

        public AuthResponse(bool success, string message, string token) : base(success, message)
        {
            this.Token = token;
        }
    }
}
