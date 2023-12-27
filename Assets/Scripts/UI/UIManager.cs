using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour, IAuthUI, IGameUI, IVillageUI, IGeneralUI
{
    public static UIManager Instance { get; private set; }

    [ToolboxItem("Auth UI")]
    [SerializeField] private GameObject _authPanel;
    
    [ToolboxItem("Game UI")]
    [SerializeField] private InputField _authUsername,_authPassword;
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _villagePanel;
    [ToolboxItem("Game UI")]
    [SerializeField] private Text _coinsText;
    [SerializeField] private Text _villagesOwner;

    private void Awake() {
        if(Instance == null)
            Instance = this;
    }


    public void SetActiveAuthPanel(bool active)
    {
        _authPanel.SetActive(active);

    }

    public string GetInputText(AuthInputType inputType)
    {
        return inputType switch
        {
            AuthInputType.Username => _authUsername.text,
            AuthInputType.Password => _authPassword.text,
            _ => null,
        };
    }

    public void SetUICoinsText(string value)
    {
        _coinsText.text = value;
    }

    public void HideAllPanels()
    {
        _authPanel.SetActive(false);
        _gamePanel.SetActive(false);
        _villagePanel.SetActive(false);
    }

    public void ShowVillagePanel(bool active)
    {
        HideAllPanels();
        _villagePanel.SetActive(active);
    
        _gamePanel.SetActive(!active);
    }

    public void SetVillageOwner(string s)
    {
        _villagesOwner.text = s;
    }
}
