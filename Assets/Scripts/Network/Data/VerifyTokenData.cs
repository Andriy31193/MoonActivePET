using Network.Enums;
using Newtonsoft.Json;

namespace Network.Request.Data
{
    public sealed class VerifyTokenData : RequestData
    {
        public override Endpoint NetworkEndpoint => Endpoint.VerifyToken;

        [JsonProperty("Token")]
        public string Token { get; }


        public VerifyTokenData(string token)
        {
            this.Token = token;
        }
    }
}