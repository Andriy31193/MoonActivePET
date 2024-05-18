using System.Collections.Generic;
using Network.Enums;

namespace Network.Configs
{
    public static class NetworkConfig
    {
        public const string ServerURL = "http://localhost:8080";

        public static readonly Dictionary<Endpoint, string> EndpointPaths = new Dictionary<Endpoint, string>
    {
        { Endpoint.Login, "/login" },
        { Endpoint.GetUser, "/data" },
        { Endpoint.VerifyToken, "/verify" },
        { Endpoint.GetRandomUser, "/randomplayer" },
    };
    }
}


