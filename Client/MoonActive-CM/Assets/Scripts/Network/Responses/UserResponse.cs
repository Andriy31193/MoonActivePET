using System;
using System.Linq;
using Network.Request.Data;
using Newtonsoft.Json;

namespace Network.Responses
{
    // Sealed class representing a response related to user data
    public sealed class UserResponse : Response
    {
        // Property to get the username from the response
        [JsonProperty("Username")]
        public string Username { get; }

        // Property to get an array of user fields from the response
        [JsonProperty("Fields")]
        public UserField[] UserFields { get; }

        // Indexer to retrieve a specific user field by name
        public string this[string name]
        {
            get
            {
                // Find the user field by name or throw an exception if not found
                var result = UserFields.FirstOrDefault(x => x.Name == name) ?? throw new NullReferenceException("Couldn't find user field with name: " + name);
                return result.Value;
            }
        }

        // Constructor to initialize the UserResponse object
        public UserResponse(bool success, string message, string username, params UserField[] userFields) : base(success, message)
        {
            this.Username = username;
            this.UserFields = userFields;
        }
    }
}
