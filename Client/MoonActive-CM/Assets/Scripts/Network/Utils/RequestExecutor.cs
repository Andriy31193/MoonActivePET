using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Network.Request;
using Network.Builders;

namespace Network.Utils
{
    public static class RequestExecutor
    {
        public static IEnumerator ExecuteRequest<TResponse>
        (
            NetworkRequest<TResponse> request,
            Action<TResponse> onResponse,
            Action<ErrorResponse> onError
        ) where TResponse : Response
        {

            using (UnityWebRequest unityWebRequest = request.CreateWebRequest())
            {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Request failed: {unityWebRequest.error}");
                    onError?.Invoke(ResponseBuilder.BuildErrorResponse(unityWebRequest.error));
                }
                else
                {
                    string jsonResponse = unityWebRequest.downloadHandler.text;
                    try
                    {
                        TResponse response = request.ParseResponse(jsonResponse);

                        if (!response.Success)
                            onError?.Invoke(ResponseBuilder.BuildErrorResponse("Request wasn't successful: " + response.Message));
                        else onResponse?.Invoke(response);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error parsing response: {ex.Message}");
                        onError?.Invoke(ResponseBuilder.BuildErrorResponse("Error parsing response"));
                    }
                }
            }
        }
    }
}
