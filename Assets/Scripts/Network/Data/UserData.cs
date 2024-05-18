using Network.Enums;
using Newtonsoft.Json;

namespace Network.Request.Data
{
    public class UserData : RequestData
    {
        public override Endpoint NetworkEndpoint => Endpoint.GetUser;

        [JsonProperty("Username")]
        public string Username { get; }

        [JsonProperty("Fields")]
        public UserField[] UserFields { get; }


        public UserData(string username, params UserField[] userFields)
        {
            this.Username = username;
            this.UserFields = userFields;
        }
    }
}