using System;

public static class NetworkUtility
{
    public static string GetFullPath(NetworkEndpoint endpoint)
    {
        if (NetworkConfig.EndpointPaths.TryGetValue(endpoint, out var path))
        {
            return NetworkConfig.ServerURL + path;
        }
        else
        {
            throw new ArgumentException($"Endpoint for type '{endpoint}' not found in the dictionary.");
        }
    }
}
