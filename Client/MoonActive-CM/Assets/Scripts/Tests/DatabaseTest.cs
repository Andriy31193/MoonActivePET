using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseTest : MonoBehaviour
{
    private const string serverURL = "http://localhost:8080/login";

    void Start()
    {
        // Simulate user login
        string username = "test";
        string password = "1234";

        // Send the login request
        StartCoroutine(SendLoginRequest(username, password));
    }

    IEnumerator SendLoginRequest(string username, string password)
    {
        // Create a JSON object with login credentials
        LoginRequest loginRequest = new LoginRequest(username, password);
        string jsonRequest = JsonUtility.ToJson(loginRequest);

        // Set up the UnityWebRequest
        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, jsonRequest))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            yield return www.SendWebRequest();

            // Check for errors
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                // Parse and handle the server's response
                string response = www.downloadHandler.text;
                Debug.Log("Server response: " + response);
            }
        }
    }

    [System.Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;

        public LoginRequest(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
