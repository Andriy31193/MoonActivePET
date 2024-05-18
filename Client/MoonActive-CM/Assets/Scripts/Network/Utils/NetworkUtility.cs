using System;
using Network.Configs;
using Network.Enums;

namespace Network.Utils
{
    public static class NetworkUtility
    {
        public static string GetFullPath(Endpoint endpoint)
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
}
