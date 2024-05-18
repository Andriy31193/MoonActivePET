using UnityEngine;
using System;
using Network.Utils;
using Network.Request;

namespace Network
{
    public sealed class NetworkBridge : MonoBehaviour
    {
        public void SendRequest<TResponse>(
            NetworkRequest<TResponse> request,
            Action<TResponse> onSuccess,
            Action<ErrorResponse> onError)
            where TResponse : Response
            => StartCoroutine(RequestExecutor.ExecuteRequest(request, onSuccess, onError));
    }
}
