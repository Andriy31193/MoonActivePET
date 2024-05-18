using System;
using System.Collections.Generic;
using System.ComponentModel;
using Network.Request.Data;
using Network.Responses;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public enum UIElementType
{
    Auth,
    MainMenu,
    Village,
}
[System.Serializable]
public class UIElementReference
{
    public UIElementType elementType;
    public GameObject uiElement;
}
public sealed class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private UIElementReference[] _uiElements;


    [ToolboxItem("Auth UI")]
    [SerializeField] private GameObject _authPanel;

    [ToolboxItem("Game UI")]
    [SerializeField] private InputField _authUsername, _authPassword;
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _villagePanel;
    [ToolboxItem("Game UI")]
    [SerializeField] private Text _coinsText;
    [SerializeField] private Text _villagesOwner;

    private List<IUIAuthenticationObserver> _authenticationObservers = new List<IUIAuthenticationObserver>();



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        SubscribeToEvents();
    }





    #region Button Handlers
    public void HandleLoginButtonClick() => NotifyAuthenticationObserversLoginButtonPressed();
    #endregion

    #region Events
    private void UpdatePlayerUI(UserResponse data)
    {
        _coinsText.text = data["Coins"];
    }
    private void UpdateVillageUI(UserResponse data)
    {
        _villagesOwner.text = data.Username + "'s village";
    }
    #endregion
    
    #region Events Subscribtions
    public void SubscribeToAuthenticationEvents(IUIAuthenticationObserver observer) =>
        _authenticationObservers.Add(observer);
    private void NotifyAuthenticationObserversLoginButtonPressed() => 
    _authenticationObservers.ForEach(observer => observer.OnLoginButtonPressed(_authUsername.text, _authPassword.text));

    private void SubscribeToEvents()
    {
        GameManager.OnPlayerDataUpdated += UpdatePlayerUI;
        VillageManager.OnVillageOwnerChanged += UpdateVillageUI;
    }
    private void UnSubscribeFromEvents()
    {
        GameManager.OnPlayerDataUpdated -= UpdatePlayerUI;
    }
    #endregion

    #region Static General 
    public static void SetActive(UIElementType elementType, bool state)
    {
        bool result = false;
        foreach(var item in Instance._uiElements)
        {
            item.uiElement.SetActive(item.elementType == elementType);
            
            if(item.elementType == elementType)
                result = true;

        }

        if (result == false)
        {
            Debug.LogError("UI Element reference not found for type: " + elementType);
            return;
        }
    }
    #endregion
    
    private void OnDestroy() => UnSubscribeFromEvents();
}
