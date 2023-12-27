using System.Collections.Generic;

public static class NetworkConfig
{
    public const string ServerURL = "http://localhost:8080";

    public static readonly Dictionary<NetworkEndpoint, string> EndpointPaths = new Dictionary<NetworkEndpoint, string>
    {
        { NetworkEndpoint.Login, "/login" }, 
    };
}


