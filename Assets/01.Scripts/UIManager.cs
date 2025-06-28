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
    [SerializeField] private GameObject _rulePanel;
    [SerializeField] private GameObject _feverPanel;

    [SerializeField] private GameObject overCat;
    
    private GameManager gm;
    private ItemManager im;
    private ObjectPoolManager pool;
    private ShopUI si;
    private AudioManager am;
    

    public bool IsPaused { get; private set; }
    
    private void Start()
    {
        gm = GameManager.Instance;
        pool = ObjectPoolManager.Instance;
        am = AudioManager.Instance;
        
        _homePanel.SetActive(true);
        am.OnBgmPlay(0);
        
        //InitSetting();
        im = FindFirstObjectByType<ItemManager>();
        si = _shopPanel.GetComponent<ShopUI>();
        
    }

    public void InitSetting()
    {
        CloseOverPanel();
        ClosePausePanel();
        CloseSettingPanel();
    }

    public void OpenRulePanel()
    {
        am.OnSfxPlay(4);
        _rulePanel.SetActive(true);
    }

    public void CloseRulePanel()
    {
        am.OnSfxPlay(4);
        _rulePanel.SetActive(false);
    }
    
    
    public void OpenSettingPanel()
    {
        am.OnSfxPlay(4);
        PauseInit();
        _settingPanel.SetActive(true);
        
    }

    public void OpenPausePanel()
    {
        am.OnSfxPlay(4);
        PauseInit();
        _pausePanel.SetActive(true);
    }

    public void OpenOverPanel()
    {
        PauseInit();
        _overPanel.SetActive(true);

        Animator animator = overCat.GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("Over");
        am.OnBgmPlay(2);

    }

    public void OpenFeverPanel()
    {
        _feverPanel.SetActive(true);
        var pos = _feverPanel.GetComponent<RectTransform>();
        pos.anchoredPosition = new Vector2(-1100f, 0);
        pos.DOAnchorPos(Vector2.zero, 1.5f)
            .SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void CloseFeverPanel()
    {
        _feverPanel.SetActive(false);
    }

    public void OpenShopPanel()
    {
        am.OnSfxPlay(4);
        si.SaveCat();
        _shopPanel.SetActive(true);
    }

    public void OpenRegisterPanel()
    {
        am.OnSfxPlay(4);
        _registerPanel.SetActive(true);
    }

    public void OpenChoicePanel()
    {
        am.OnSfxPlay(4);
        _choicePanel.SetActive(true);
    }

    public void CloseChoicePanel()
    {
        am.OnSfxPlay(4);
        _choicePanel.SetActive(false);
    }

    public void CloseRegisterPanel()
    {
        am.OnSfxPlay(4);
        _registerPanel.SetActive(false);
    }

    public void CloseShopPanel()
    {
        am.OnSfxPlay(4);
        _shopPanel.SetActive(false);
    }

    public void CloseOverPanel()
    {
        am.OnSfxPlay(4);
        _overPanel.SetActive(false);
    }

    public void ClosePausePanel()
    {
        am.OnSfxPlay(4);
        _pausePanel.SetActive(false);
        IsPaused = false;
        gm.resumed = true;
        Time.timeScale = 1f;
    }
    
    public void CloseSettingPanel()
    {
        am.OnSfxPlay(4);
        _settingPanel.SetActive(false);
    }


    public void GoHome()
    {
        am.OnSfxPlay(4);
        am.OnBgmPlay(0);
        CloseOverPanel();
        _homePanel.SetActive(true);
    }

    public void GoTry()
    {
        am.OnBgmPlay(3);
        gm.StopAllCoroutines();
        pool.ResetPool();
        if (gm.InitSetting() == -1)
        {
            OpenChoicePanel();
            return;
        }
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
        am.OnSfxPlay(4);
        bool anyUnlocked = false;

        for (int i = 0; i < 4; i++)
        {
            int k = PlayerPrefs.GetInt("catUnlocked_" + i);
            if (k == 1)
            {
                anyUnlocked = true;
                break;
            }
        }

        if (anyUnlocked)
        {
            GoTry();
            _homePanel.SetActive(false);
        }
        else
        {
           Debug.Log("null");
           OpenChoicePanel();
        }
    }

}
