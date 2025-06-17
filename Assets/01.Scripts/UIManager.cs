using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private GameObject _pausePanel;
    
    
    private GameManager gm;

    public bool IsPaused { get; private set; }
    
    private void Start()
    {
        gm = GameManager.Instance;
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

    public void PauseInit()
    {
        IsPaused = true;
        Time.timeScale = 0f;
    }
}
