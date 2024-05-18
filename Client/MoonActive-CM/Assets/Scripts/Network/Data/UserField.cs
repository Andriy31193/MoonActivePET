using Network.Enums;
using Newtonsoft.Json;

namespace Network.Request.Data
{
    public class UserField
    {
        [JsonProperty("Operation")]
        public string Operation { get; }

        [JsonProperty("Name")]
        public string Name { get; }
        
        [JsonProperty("Value")]
        public string Value { get; }
        public UserField(string name, OperationType operationType, string value)
        {
            this.Name = name;
            this.Operation = operationType.ToString();
            this.Value = value;
        }
    }
}