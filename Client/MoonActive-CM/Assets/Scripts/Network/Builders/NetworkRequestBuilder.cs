using Network.Enums;
using Network.Request;
using Network.Request.Data;

namespace Network.Builders
{
    public class NetworkRequestBuilder
    {
        public static NetworkRequest<TResponse> BuildRequest<TRequestData, TResponse>(TRequestData requestData) 
            where TRequestData : RequestData
            where TResponse : Response
        {
            return new NetworkRequest<TResponse>(requestData);
        }
                public static UserField BuildUserField(string key, string value, OperationType operation)
        {
            return null;
        }
    }
}
