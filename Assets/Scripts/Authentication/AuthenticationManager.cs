using System;
using System.Collections;
using Network.Responses;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class AuthenticationManager : MonoBehaviour
{


    private IAuthUI _ui;

    private void Start()
    {
        _ui = UIManager.Instance;

        _ui.SetActiveAuthPanel(true);

        string token = PlayerPrefs.GetString("JWTToken", "");

        NetworkManager.TryVerifyToken(token, OnTokenVerificationSuccess, OnTokenVerificationFailure);
    }

    public void TryLoginOnClick()
    {
        NetworkManager.TryLogin(_ui.GetInputText(AuthInputType.Username), _ui.GetInputText(AuthInputType.Password), OnAuthenticationResponse);
    }

    #region Token Verification callbacks
    private void OnTokenVerificationSuccess(string token) => OnUserAuthorized(new LoginResponse(){Success = true, Token = token});
    private void OnTokenVerificationFailure(string response)
    {
        Debug.LogWarning(response);
    }
    #endregion

        #region Token Verification callbacks
    private void OnAuthenticationResponse(LoginResponse response) => OnUserAuthorized(response);

    private void OnUserAuthorized(LoginResponse loginResponse)
    {
        if(loginResponse.Token.Contains(" "))
            loginResponse.Token = loginResponse.Token.Split(' ')[1];

        PlayerPrefs.SetString("JWTToken", loginResponse.Token);
        
        _ui.SetActiveAuthPanel(false);
        
        GameManager.Instance.OnUserAuthorized(loginResponse.Token);
    }
    #endregion

}
