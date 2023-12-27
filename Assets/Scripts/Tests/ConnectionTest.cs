using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

[System.Serializable]
public class AttackData
{
    public int attackAmount;
}

public class ConnectionTest : MonoBehaviour
{
    private const string serverURL = "http://localhost:32769";

    private void Start()
    {
        // Simulating an attack amount
        int attackAmount = 50;

        // Send the attack request
        StartCoroutine(SendAttackRequest(attackAmount));
    }

    private IEnumerator SendAttackRequest(int attackAmount)
    {
        // Create an instance of the AttackData class
        AttackData attackData = new AttackData
        {
            attackAmount = attackAmount
        };

        // Convert the data structure to a JSON string
        string requestData = JsonUtility.ToJson(attackData);

        // Send a POST request to the server
        using (UnityWebRequest www = UnityWebRequest.Post($"{serverURL}/api/game/attack", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(requestData);
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
                // Parse and display the server's response
                Debug.Log("Server response: " + www.downloadHandler.text);
            }
        }
    }
}
