using UnityEngine;
using System;
using Network.Responses;
using Network.Request.Data;
using Network.Builders;

namespace Network
{
    public sealed class NetworkManager : MonoBehaviour
    {
        private static NetworkBridge _bridge;

        [RuntimeInitializeOnLoadMethod]
        private static void Start()
        {
            GameObject _networkingPeer = new GameObject("NetworkBridge");

            DontDestroyOnLoad(_networkingPeer);

            _networkingPeer.hideFlags = HideFlags.HideInHierarchy;
            _networkingPeer.AddComponent(typeof(NetworkBridge));

            _bridge = _networkingPeer.GetComponent<NetworkBridge>();
        }

        public static void TryVerifyToken(string token, Action<AuthResponse> onSuccess, Action<ErrorResponse> onError)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                onError?.Invoke(ResponseBuilder.BuildErrorResponse("Token has incorrect format."));
                return;
            }

            SendRequest(new VerifyTokenData(token), onSuccess, onError);
        }
        public static void TryLogin(string username, string password, Action<AuthResponse> onSuccess, Action<ErrorResponse> onError)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                onError?.Invoke(ResponseBuilder.BuildErrorResponse("Username or password is empty or null."));
                return;
            }

            SendRequest(new AuthData(username, password), onSuccess, onError);
        }
        public static void TrySendPlayerData(UserData userData, Action<UserResponse> onSuccess, Action<ErrorResponse> onError)
        {
            if (userData == null)
            {
                onError?.Invoke(ResponseBuilder.BuildErrorResponse("User is empty or null."));
                return;
            }

            SendRequest(userData, onSuccess, onError);
        }

        public static void TryGetRandomPlayer(string user, Action<UserResponse> onSuccess, Action<ErrorResponse> onError)
        {
            if (string.IsNullOrWhiteSpace(user))
            {
                onError?.Invoke(ResponseBuilder.BuildErrorResponse("Username is null or empty!"));
                return;
            }
            SendRequest(new RandomUserData(user), onSuccess, onError);
        }

        #region Senders
        private static void SendRequest<TResponse, TRequestData>(TRequestData data, Action<TResponse> onSuccess, Action<ErrorResponse> onError)
        where TResponse : Response
        where TRequestData : RequestData
        {
            var request = NetworkRequestBuilder.BuildRequest<TRequestData, TResponse>(data);
            _bridge.SendRequest(request, onSuccess, onError);
        }
        #endregion
    }
}