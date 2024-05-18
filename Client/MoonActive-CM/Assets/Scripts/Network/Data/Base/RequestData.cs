using Network.Enums;

namespace Network.Request.Data
{
    public abstract class RequestData
    {
        public abstract Endpoint NetworkEndpoint { get; }
    }
}