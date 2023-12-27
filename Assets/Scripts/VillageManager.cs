using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class VillageManager : MonoBehaviour
{

    public static VillageManager Instance {get; private set; }
    [SerializeField] private Transform _villageContainer;

    [SerializeField] private GameObject _villageHousePrefab;

    private IVillageUI _ivillageUI;

    private List<GameObject> _houses = new List<GameObject>();

    private UserData villageOwner;

    private void Awake() {
     Instance = this;   
    }

    private void Start() {
        _ivillageUI = UIManager.Instance;
    }
    public void LoadVillage(string username, VerificationType type)
    {
        var r = new VerificationRequest(username, type);
        LoadData(username, r);
    }
    private void LoadData(string u, VerificationRequest r)
    {
        //NetworkManager.TryGetPlayerData(r, OnPlayerDataReceived);
    }
    private void BuildVillage(int amount)
    {
        for(int i = 0; i < _houses.Count; i++)
            Destroy(_houses[i].gameObject);
        
        _houses.Clear();

        _houses.AddRange(Build(_villageContainer, _villageHousePrefab, amount / 1000));
        _ivillageUI.ShowVillagePanel(true);
    }
    public void AttackVillage()
    {
        if(GameManager.Instance.GetLocalPlayerUsername() != villageOwner.username && SlotMachine.Hammers > 0)
        {
            SlotMachine.Hammers--;
            VerificationRequest v = new VerificationRequest(villageOwner.username, VerificationType.Username);
            NetworkManager.TryUpdateCoins(v, "decrease", 1000, OnResponseAttack);
        }
    }
    private void OnResponseAttack(bool s, string n)
    {
        LoadVillage(villageOwner.username, VerificationType.Username);
    }
public void Back()
{
    _ivillageUI.ShowVillagePanel(false);
}
    public GameObject[] Build(Transform parent, GameObject prefab, int count)
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

    private void OnPlayerDataReceived(Network.Responses.UserDataResponse userData)
    {
        if (userData.Success)
        {
            villageOwner = userData.UserData;

            _ivillageUI.SetVillageOwner(villageOwner.username+"'s village");
            BuildVillage(villageOwner.coins);
        }
    }
}
