using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Network.Responses;
using System.Collections.Generic;

public sealed class NetworkPeer : MonoBehaviour
{
    private const string serverURL = "http://localhost:8080";

    public void VerifyToken(string token, Action<string> onSuccess, Action<string> onFailure) => StartCoroutine(IEVerifyToken(token, onSuccess, onFailure));
    public void Login(string username, string password, Action<LoginResponse> onResponse) => StartCoroutine(IESendLoginRequest(username, password, onResponse));
    public void GetPlayerData(VerificationRequest r, Action<UserDataResponse> onResponse) => StartCoroutine(IEGetPlayerData(r, onResponse));
    public void UpdateCoins(VerificationRequest r, string operation, int amount, Action<bool, string> onResponse) => StartCoroutine(IEAddOrDecreaseCoins(r,operation,amount,onResponse));
    public void GetRndPlayer(string nickname, Action<bool, string> onResponse) => StartCoroutine(IEGetRndPlayer(nickname, onResponse));


    private IEnumerator LoginAndAttack()
    {
        // Simulating a login request
        LoginResponse loginResponse = new LoginResponse();
        yield return StartCoroutine(IESendLoginRequest("test", "1234", response => loginResponse = response));

        // Check if the login was successful
        if (loginResponse.Success)
        {
            // Use the obtained token for the attack request
           // yield return StartCoroutine(IESendAttackRequest(loginResponse.Token, response => Debug.Log(response), error => Debug.LogError(error)));
        }
        else
        {
            Debug.LogError("Login failed");
        }
    }

    private IEnumerator IESendLoginRequest(string username, string password, Action<LoginResponse> onResponse)
    {
        yield return null;
            // Create a login request object
            // LoginRequest loginRequest = new LoginRequest
            // {
            //     Username = username,
            //     Password = password
            // };

            // Convert the login request object to JSON
            // string jsonRequest = JsonUtility.ToJson(loginRequest);

            // // Create a UnityWebRequest for the login request
            // using (UnityWebRequest www = UnityWebRequest.Post($"{serverURL}/login", "POST"))
            // {
            //     // Set the request headers
            //     www.SetRequestHeader("Content-Type", "application/json");

            //     // Attach the JSON data to the request
            //     byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
            //     www.uploadHandler = new UploadHandlerRaw(bodyRaw);

            //     // Send the login request and wait for the response
            //     yield return www.SendWebRequest();

            //     // Check for errors
            //     if (www.result != UnityWebRequest.Result.Success)
            //     {
            //         Debug.LogError("Error: " + www.error);
            //     }
            //     else
            //     {
            //         // Parse and handle the server's response
            //         string response = www.downloadHandler.text;
            //         Debug.Log("Server response: " + response);

            //         if (!string.IsNullOrEmpty(response))
            //         {
            //             try
            //             {
            //                 LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(response);

            //                 onResponse?.Invoke(loginResponse);
            //             }
            //             catch (Exception e)
            //             {
            //                 Debug.LogWarning("JSON: " + e.Message);
            //             }
            //         }
            //         else
            //         {
            //             Debug.LogWarning("Server response is empty");
            //         }
            //     }

            // }
    }
    private IEnumerator IEGetPlayerData(VerificationRequest r, Action<UserDataResponse> onResponse)
    {
        using (UnityWebRequest request = UnityWebRequest.Post($"{serverURL}/data", "POST"))
        {

// public class UserData
// {
//     public string username;
//     public string password;
//     public int coins;
// }



            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", r.GetRequest());

            // Send the request
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                // Parse and handle the server's response
                string response = request.downloadHandler.text;
                Debug.Log("Server response: " + response);

                // Deserialize the JSON response
                UserDataResponse userData = JsonUtility.FromJson<UserDataResponse>(response);

                // Access the user data
                if (userData != null && userData.Success)
                {
                    UserData userInfo = userData.UserData;
                    onResponse?.Invoke(userData);
                }
                else
                {
                    Debug.LogError("Failed to deserialize user data from the server response.");
                }
            }

        }
        onResponse?.Invoke(new UserDataResponse());
    }
    // Update your SendAttackRequest method
    private IEnumerator IESendAttackRequest(VerificationRequest request, Action<string> onSuccess, Action<string> onError)
    {
        // // Create an attack request object
        // AttackRequest attackRequest = new AttackRequest();

        // // Convert the attack request object to JSON
        // string jsonRequest = JsonUtility.ToJson(attackRequest);

        // // Create a UnityWebRequest for the attack request
        // using (UnityWebRequest www = UnityWebRequest.Post($"{serverURL}/attack", "POST"))
        // {
        //     // Set the request headers
        //     www.SetRequestHeader("Content-Type", "application/json");
        //     www.SetRequestHeader("Authorization", request.GetRequest());  // Include "Bearer" prefix

        //     // Attach the JSON data to the request
        //     byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
        //     www.uploadHandler = new UploadHandlerRaw(bodyRaw);

        //     // Send the attack request and wait for the response
        //     yield return www.SendWebRequest();

        //     // Check for errors
        //     if (www.result != UnityWebRequest.Result.Success)
        //     {
        //         Debug.LogError("Error: " + www.error);
        //         onError?.Invoke(www.error);
        //     }
        //     else
        //     {
        //         // Parse and handle the server's response
        //         string response = www.downloadHandler.text;
        //         Debug.Log("Server response: " + response);

        //         // Invoke the callback with the response
        //         onSuccess?.Invoke(response);
        //     }
        // }
        yield return null;
    }

        private IEnumerator IEGetRndPlayer(string user, Action<bool, string> responsed)
    {
        // Create an attack request object

        // Convert the attack request object to JSON
string jsonPayload = "{\"nickname\": \"" +user + "\"}";

        // Create a UnityWebRequest for the attack request
        using (UnityWebRequest www = UnityWebRequest.Post($"{serverURL}/randomplayer", "POST"))
        {

            // Set the request headers
            www.SetRequestHeader("Content-Type", "application/json");

            // Attach the JSON data to the request
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);

            // Send the attack request and wait for the response
            yield return www.SendWebRequest();

            // Check for errors
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
                responsed?.Invoke(false, www.error);
            }
            else
            {
                // Parse and handle the server's response
                string response = www.downloadHandler.text;
                Debug.Log("Server response: " + response);

                // Invoke the callback with the response
                responsed?.Invoke(true, response);
            }
        }
    }
    [System.Serializable]
    private class payload
    {
        public string username;
        public string operation;
        public int amount;
    }
    private IEnumerator IEAddOrDecreaseCoins(VerificationRequest r, string operation, int amount, System.Action<bool, string> onResponse)
    {
        // Create the request payload
        payload p = new payload(){username="", operation=operation, amount = amount};

        // Convert the payload to JSON
        string jsonPayload = JsonUtility.ToJson(p);
        UnityWebRequest request = new UnityWebRequest(serverURL+"/add-coins", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", r.GetRequest());  // Include "Bearer" prefix

        // Send the request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Successfully received response
            string response = request.downloadHandler.text;
            Debug.Log("Server response: " + response);

            // Handle the server response
            onResponse?.Invoke(true, response);
        }
        else
        {
            // Error in the request
            Debug.LogError("Error in request: " + request.error);
            onResponse?.Invoke(false, "Error in request: " + request.error);
        }
    }

    private IEnumerator IEVerifyToken(string token, Action<string> onSucess, Action<string> onFailure)
    {
        // Create a UnityWebRequest to verify the token
        using (UnityWebRequest www = UnityWebRequest.Get($"{serverURL}/verify?token=Bearer {token}"))
        {
            // Send the verification request and wait for the response
            yield return www.SendWebRequest();

            // Check for errors
            string response = www.downloadHandler.text;
            Debug.Log("Server response: " + response);
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error verifying token: " + www.error);
                onFailure?.Invoke(www.error);
            }
            else
            {
                onSucess?.Invoke(response);
            }
        }
    }


}
