using System.Collections.Generic;
using Network;
using Network.Responses;
using UnityEngine;

public class AuthenticationManager : MonoBehaviour, IUIAuthenticationObserver
{
    public static AuthenticationManager Instance { get; private set; }

    private List<IAuthenticationObserver> _observers = new List<IAuthenticationObserver>();
    private void Awake()
    {
        Instance = Instance ?? this;

        if (Instance != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        SubscribeToAuthenticationEvents();
        TryVerifyToken(PlayerPrefs.GetString("JWTToken", ""));
    }

    #region UI Events
    public void OnLoginButtonPressed(string username, string password)=> NetworkManager.TryLogin(username, password, OnAuthenticationSuccess, OnAuthenticationFailure);
    #endregion


    #region Token Verification callbacks
    private void TryVerifyToken(string token)
    {
        NetworkManager.TryVerifyToken(token, OnAuthenticationSuccess, OnAuthenticationFailure);
    }
    private void OnAuthenticationFailure(ErrorResponse response)
    {
        Debug.LogWarning(response);
    }
    private void OnAuthenticationSuccess(AuthResponse response)
    {
        if (response.Success)
        {
            PlayerPrefs.SetString("JWTToken", response.Token);

            NotifyObserversUserLoggedIn(response.Token);

            UIManager.SetActive(UIElementType.Auth, false);
        }
        else Debug.LogError("Authentication response wasn't successfull.");
    }
    #endregion

    #region Observer
    public void Subscribe(IAuthenticationObserver observer) => _observers.Add(observer);
    private void NotifyObserversUserLoggedIn(string token) => _observers.ForEach(x => x.OnUserLoggedIn(token));
    #endregion

    #region Events
    private void SubscribeToAuthenticationEvents()
    {
        UIManager.Instance.SubscribeToAuthenticationEvents(this);   
    }
    #endregion

}
