using System;
using System.Collections.Generic;
using Network;
using Network.Builders;
using Network.Enums;
using Network.Request.Data;
using Network.Responses;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Temporary version of VillageManager
/// </summary>


public class VillageManager : MonoBehaviour
{
    public static event Action<UserResponse> OnVillageOwnerChanged;
    public static VillageManager Instance { get; private set; }

    [SerializeField] private Transform _villageContainer;

    [SerializeField] private GameObject _villageHousePrefab;


    private UserResponse _villageOwner;
    private bool _attacked = false;

    public UserResponse VillageOwner
    {
        get => _villageOwner;
        set
        {
            if (_villageOwner != value)
            {
                _villageOwner = value;
                
                OnLocalVillageOwnerChanged();
            }
        }
    }
    private List<GameObject> _uiHouses = new List<GameObject>();

    private void Awake() {

        if(Instance == null)
            Instance = this;   
        else Destroy(this.gameObject);
    }

    private void Start() {
        GameManager.OnPlayerDataUpdated += LoadData;
        SlotMachine.OnStartAttack += LoadData;
    }
    public void LoadRandomPlayerVillage()
    {
        NetworkManager.TryGetRandomPlayer(GameManager.Instance.GetLocalPlayerToken(), OnRandomPlayerReceived, OnRandomPlayerReceiveFailed);
    }
    private void OnRandomPlayerReceived(UserResponse response)
    {
        VillageOwner = response;
        UIManager.SetActive(UIElementType.Village, true);
    }
    private void OnRandomPlayerReceiveFailed(ErrorResponse response) => Debug.LogError(response.Message);
    private void LoadData(UserResponse user) => VillageOwner = user;
    public void LoadLocalPlayerVillage()
    {
        UserRequestDataBuilder builder = new UserRequestDataBuilder(GameManager.Instance.GetLocalPlayerUsername());
        builder.AddField("Coins", OperationType.GET);

        NetworkManager.TrySendPlayerData(builder.Build(), OnPlayerDataReceived, OnPlayerDataReceiveFailed);

        UIManager.SetActive(UIElementType.Village, true);
    }
    public void BuildVillage(int coins)
    {
        for(int i = 0; i < _uiHouses.Count; i++)
            Destroy(_uiHouses[i].gameObject);
        
        _uiHouses.Clear();

        _uiHouses.AddRange(BuildUI(_villageContainer, _villageHousePrefab, coins / 1000));
    }
    private void AttackVillage()
    {
        
        if(GameManager.Instance.GetLocalPlayerUsername() != _villageOwner.Username && _attacked == false)
        {
            UserRequestDataBuilder builder = new UserRequestDataBuilder(_villageOwner.Username);
            builder.AddField("Coins", OperationType.DECREASE, "1000");

            NetworkManager.TrySendPlayerData(
                builder.Build(),
                (UserResponse data) => 
                {
                    _attacked = true;
                    OnPlayerDataReceived(data);
                    
                },
                OnPlayerDataReceiveFailed);
        }
    }
    public void Back()
    {
        _attacked = false;
        UIManager.SetActive(UIElementType.MainMenu, true);
    }
    private GameObject[] BuildUI(Transform parent, GameObject prefab, int count)
    {
        GameObject[] result = new GameObject[count];
        for(int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(prefab, parent, false);
            go.GetComponent<Button>().onClick.AddListener(AttackVillage);
            result[i] = go;
        }

        return result;
    }

    private void OnLocalVillageOwnerChanged()
    {
        bool parseResult = int.TryParse(_villageOwner["Coins"], out int coins);

        if (parseResult == false)
        {
            Debug.Log("Couldn't parse Coins value.");
            return;
        }

        OnVillageOwnerChanged?.Invoke(_villageOwner);
        BuildVillage(coins);
    
    }
    private void OnPlayerDataReceived(UserResponse userData)
    {
        if (userData.Success) 
            LoadData(userData);
        else Debug.LogError("Request wasn't successfull.");
    }
    private void OnPlayerDataReceiveFailed(ErrorResponse error) => Debug.LogError(error.Message);
}
