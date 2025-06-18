using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _homePanel;
    [SerializeField] private GameObject _overPanel;
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private GameObject _registerPanel;
    [SerializeField] private GameObject _choicePanel;
    
    private GameManager gm;
    private ItemManager im;
    private ObjectPoolManager pool;
    private ShopUI si;

    public bool IsPaused { get; private set; }
    
    private void Start()
    {
        gm = GameManager.Instance;
        pool = ObjectPoolManager.Instance;
        GoHome();
        
        InitSetting();
        im = FindFirstObjectByType<ItemManager>();
        si = _shopPanel.GetComponent<ShopUI>();
        
    }

    public void InitSetting()
    {
        CloseOverPanel();
        ClosePausePanel();
        CloseSettingPanel();
    }
    
    
    public void OpenSettingPanel()
    {
        PauseInit();
        _settingPanel.SetActive(true);
        
    }

    public void OpenPausePanel()
    {
        PauseInit();
        _pausePanel.SetActive(true);
    }

    public void OpenOverPanel()
    {
        PauseInit();
        _overPanel.SetActive(true);
    }

    public void OpenShopPanel()
    {
        si.SaveCat();
        _shopPanel.SetActive(true);
    }

    public void OpenRegisterPanel()
    {
        _registerPanel.SetActive(true);
    }

    public void OpenChoicePanel()
    {
        _choicePanel.SetActive(true);
    }

    public void CloseChoicePanel()
    {
        _choicePanel.SetActive(false);
    }

    public void CloseRegisterPanel()
    {
        _registerPanel.SetActive(false);
    }

    public void CloseShopPanel()
    {
        _shopPanel.SetActive(false);
    }

    public void CloseOverPanel()
    {
        _overPanel.SetActive(false);
    }

    public void ClosePausePanel()
    {
        _pausePanel.SetActive(false);
        IsPaused = false;
        gm.resumed = true;
        Time.timeScale = 1f;
    }
    
    public void CloseSettingPanel()
    {
        _settingPanel.SetActive(false);
    }


    public void GoHome()
    {
        CloseOverPanel();
        _homePanel.SetActive(true);
    }

    public void GoTry()
    {
        gm.StopAllCoroutines();
        gm.InitSetting();
        pool.ResetPool();
        pool.SpawnCatSetting(gm._level);
        InitSetting();
        im.InitSetting();
        
        DOTween.KillAll();
        
    }

    public void PauseInit()
    {
        IsPaused = true;
        Time.timeScale = 0f;
    }


    public void GoGame()
    {
        if (gm._level == null)
        {
            Debug.Log("null");
            OpenChoicePanel();
            return;
        }
        GoTry();
        _homePanel.SetActive(false);
    }
}
