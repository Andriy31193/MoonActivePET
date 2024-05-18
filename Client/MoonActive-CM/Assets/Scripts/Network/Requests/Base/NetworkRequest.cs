using Network.Builders;
using Network.Request.Data;
using Network.Utils;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Network.Request
{
    public class NetworkRequest<TResponse> where TResponse : Response
    {
        public RequestData Data { get; }

        public NetworkRequest(RequestData data)
        {
            this.Data = data;
        }

        public virtual UnityWebRequest CreateWebRequest()
        {
            UnityWebRequestBuilder builder = new UnityWebRequestBuilder(NetworkUtility.GetFullPath(this.Data.NetworkEndpoint), "POST");
            builder.SetHeader("Content-Type", "application/json");
            builder.SetBody(JsonConvert.SerializeObject(this.Data));
            return builder.Build();
        }

        public TResponse ParseResponse(string jsonResponse) => JsonConvert.DeserializeObject<TResponse>(jsonResponse);
    }
}