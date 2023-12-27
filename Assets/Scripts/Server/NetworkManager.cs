using UnityEngine;
using System;
using Network.Responses;

public sealed class NetworkManager : MonoBehaviour
{
    private static NetworkPeer _peer;
    [RuntimeInitializeOnLoadMethod]
    private static void Start()
    {
        GameObject _networkingPeer = new GameObject("NetworkPeer");
        DontDestroyOnLoad(_networkingPeer);
        _networkingPeer.hideFlags = HideFlags.HideInHierarchy;
        _networkingPeer.AddComponent(typeof(NetworkPeer));
        _peer = _networkingPeer.GetComponent<NetworkPeer>();
    }

    public static void TryVerifyToken(string token, Action<string> onSuccess, Action<string> onFailure)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            onFailure?.Invoke("Token has incorrect format.");
            return;
        }

        _peer.VerifyToken(token, onSuccess, onFailure);
    }
    public static void TryLogin(string username, string password, Action<LoginResponse> response)
    {
        if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            response?.Invoke(new LoginResponse() { Success = false });
            Debug.LogError("Username or password is empty or null.");
            return;
        }

        _peer.Login(username, password, response);
    }
    public static void TryGetPlayerData(VerificationRequest r, Action<UserDataResponse> response) 
    {
        if(string.IsNullOrWhiteSpace(r.GetRequest()))
        {
            response?.Invoke(null);
            Debug.LogError("Username or password is empty or null.");
            return;
        }

        _peer.GetPlayerData(r, response);
    }
    public static void TryUpdateCoins(VerificationRequest r, string operation, int amount, Action<bool, string> response) 
    {
        if(string.IsNullOrWhiteSpace(r.GetRequest()) || string.IsNullOrWhiteSpace(operation))
        {
            response?.Invoke(false, null);
            Debug.LogError("Token or operation is null.");
            return;
        }

        _peer.UpdateCoins(r, operation, amount, response);
    }

    public static void GetRndPlayer(string user, Action<bool, string> action)
    {
        if(string.IsNullOrWhiteSpace(user))
        {
            action?.Invoke(false, null);
            Debug.LogError("Token or operation is null.");
            return;
        }
        _peer.GetRndPlayer(user, action);
    }

}
