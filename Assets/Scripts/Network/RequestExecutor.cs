using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class RequestExecutor
{
    public IEnumerator ExecuteRequest<M, R>(Request<M, R> request, NetworkEndpoint endpoint, Action<R> onResponse, Action<ErrorResponse> onError)
    {
        
        using (UnityWebRequest unityWebRequest = request.CreateWebRequest(endpoint))
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
                    R response = request.ParseResponse(jsonResponse);
                    onResponse?.Invoke(response);
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
