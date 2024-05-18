#region Namespaces

using UnityEngine;
using Network.Responses;
using Network;
using Network.Request.Data;
using System;
using Network.Builders;
using Network.Enums;

#endregion

public delegate void OnUserAuthorized(string token);

#region GameManager Class

public sealed class GameManager : MonoBehaviour, IAuthenticationObserver
{
    #region Singleton Instance

    public static GameManager Instance { get; private set; }

    #endregion

    #region Events

    public static event Action<UserResponse> OnPlayerDataUpdated;
    public OnUserAuthorized OnUserAuthorized { get; set; }

    #endregion

    #region Private Fields

    private string _token;
    private UserResponse _data;

    #endregion

    #region Unity Lifecycle Methods

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AuthenticationManager.Instance.Subscribe(this);
    }

    #endregion

    #region IAuthenticationObserver Implementation

    public void OnUserAuthorizedd(string token)
    {
        _token = token;
        LoadData();
    }

    public void OnUserLoggedIn(string token)
    {
        Debug.Log("Logged in!");
        _token = token;
        LoadData();
    }

    #endregion

    #region Private Methods

    private void LoadData()
    {
        UserRequestDataBuilder builder = new UserRequestDataBuilder(_token);
        builder.AddField("Coins", OperationType.GET);
        NetworkManager.TrySendPlayerData(builder.Build(), OnPlayerDataReceived, OnPlayerDataReceiveFailure);
    }

    private void OnPlayerDataReceiveFailure(ErrorResponse errorResponse) => Debug.LogError(errorResponse.Message);

    public void OnPlayerDataReceived(UserResponse response)
    {
        if (response.Success)
        {
            _data = new UserResponse(response.Success, response.Message, response.Username, response.UserFields);

            if (_data == null)
            {
                Debug.LogError("Cannot display information. Data is empty!");
                return;
            }

            OnPlayerDataUpdated?.Invoke(_data);
            UIManager.SetActive(UIElementType.MainMenu, true);
        }
    }

    #endregion

    #region Public Methods

    public string GetLocalPlayerToken()
    {
        if (string.IsNullOrWhiteSpace(_token) == false)
            return _token;

        throw new NullReferenceException("Player token is null!");
    }

    public string GetLocalPlayerUsername()
    {
        if (_data != null)
            return _data.Username;

        throw new NullReferenceException("Player data is null!");
    }

    #endregion
}

#endregion
