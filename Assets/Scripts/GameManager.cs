using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private string _token;

    private UserData _data;

    private IGameUI _gameUI;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _gameUI = UIManager.Instance;


        RequestExecutor rx = new RequestExecutor();
        LoginModel loginModel = new LoginModel
        {
            Username = "test",
            Password = "1234"
        };
        Request<LoginModel, LoginResponse> request = new Request<LoginModel, LoginResponse>(loginModel);
        
        StartCoroutine(rx.ExecuteRequest(request, NetworkEndpoint.Login, onOK, onNotOK));
    }
    private void onOK(Response loginResponse)
    {
        Debug.Log(((LoginResponse)loginResponse).Success);
        Debug.Log(((LoginResponse)loginResponse).Token);
    }
    private void onNotOK(Response errorResponse)
    {
Debug.Log(errorResponse.Message);
    }

    public void OnUserAuthorized(string token)
    {
        _token = token;

        LoadData();
    }

    public void LoadMyVillage() => VillageManager.Instance.LoadVillage(_token, VerificationType.Token);
    public void UpdatePlayerCoins(int amount, bool isSubstraction = false)
    {
        var r = new VerificationRequest(_token, VerificationType.Token);
        NetworkManager.TryUpdateCoins(r, isSubstraction ? "decrease" : "add", amount, OnCoinsUpdated);
    }
    private void LoadData()
    {
        var r = new VerificationRequest(_token, VerificationType.Token);
        //NetworkManager.TryGetPlayerData(r, OnPlayerDataReceived);
    }

    private void OnPlayerDataReceived(Network.Responses.UserDataResponse userData)
    {
        if (userData.Success)
        {
            _data = userData.UserData;

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (_data == null)
        {
            Debug.LogError("Cannot display information. Data is empty!");
            return;
        }

        _gameUI.SetUICoinsText(_data.coins.ToString());
    }
    public string GetLocalPlayerUsername()
    {
        if(_data != null)
        {
            return _data.username;
        }
        return null;
    }
    private void OnCoinsUpdated(bool state, string response)
    {
        Debug.Log(response);
        LoadData();
    }
}
