using Newtonsoft.Json;
using UnityEngine.Networking;

public class Request<Model, Response>
{
    public Model Data { get; private set; }

    public Request(Model data)
    {
        this.Data = data;
    }

    public virtual UnityWebRequest CreateWebRequest(NetworkEndpoint endpoint)
    {
        UnityWebRequestBuilder builder = new UnityWebRequestBuilder(NetworkUtility.GetFullPath(endpoint), "POST");
        builder.SetHeader("Content-Type", "application/json");
        builder.SetBody(JsonConvert.SerializeObject(this.Data));
        return builder.Build();
    }

    public Response ParseResponse(string jsonResponse) => JsonConvert.DeserializeObject<Response>(jsonResponse);
}