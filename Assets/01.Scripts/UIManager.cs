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
    
    private GameManager gm;
    private ItemManager im;
    private ObjectPoolManager pool;

    public bool IsPaused { get; private set; }
    
    private void Start()
    {
        gm = GameManager.Instance;
        pool = ObjectPoolManager.Instance;
        GoHome();
        
        InitSetting();
        im = FindFirstObjectByType<ItemManager>();
        
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
        GoTry();
        _homePanel.SetActive(false);
    }
}
