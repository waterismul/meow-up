using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField couponNumber;
    [SerializeField] private Button registerButton;
    [SerializeField] private Image[] catImages;
    [SerializeField] private Image[] lockImages;
    [SerializeField] private Toggle[] catToggles;
    [SerializeField] private ToggleGroup toggleGroup;

    private ConstInfo _constInfo;
    
    private Toggle _saveToggle;

    private GameManager gm;
    private AudioManager am;

    private void Awake()
    {
        // 모든 토글을 강제로 꺼버려
        foreach (var toggle in catToggles)
        {
            toggle.isOn = false;
        }

       
    }

    public void OnToggleChanged(Toggle changedToggle)
    {
        if (changedToggle.isOn)
        {
            // 하나만 선택되도록 하고, 마지막 하나를 끄는 걸 막음
            if (toggleGroup.ActiveToggles().Count() is 0)
            {
                changedToggle.isOn = true; // 다시 켜버림
                return;
            }

            _saveToggle = changedToggle;

            // 어떤 토글이 선택됐는지 인덱스를 알아내기
            int index = Array.IndexOf(catToggles, changedToggle);

            // 인덱스에 따라 분기
            switch (index)
            {
                case 0:
                    am.OnSfxPlay(4);
                    Debug.Log("갈색 고양이 선택됨!");
                    _constInfo.CatIndexInit(0);
                    PlayerPrefs.SetInt("catUnlocked_" + 0, 1);
                    PlayerPrefs.SetInt("selectedCatIndex", 0);
                    break;
                case 1:
                    am.OnSfxPlay(4);
                    Debug.Log("검정 고양이 선택됨!");
                    _constInfo.CatIndexInit(1);
                    PlayerPrefs.SetInt("catUnlocked_" + 1, 1);
                    PlayerPrefs.SetInt("selectedCatIndex", 1);
                    break;
                case 2:
                    am.OnSfxPlay(4);
                    Debug.Log("회색 고양이 선택됨!");
                    _constInfo.CatIndexInit(2);
                    PlayerPrefs.SetInt("catUnlocked_" + 2, 1);
                    PlayerPrefs.SetInt("selectedCatIndex", 3);
                    break;
                case 3:
                    am.OnSfxPlay(4);
                    Debug.Log("샴 고양이 선택됨!");
                    _constInfo.CatIndexInit(3);
                    PlayerPrefs.SetInt("catUnlocked_" + 3, 1);
                    PlayerPrefs.SetInt("selectedCatIndex", 3);
                    break;
                default:
                    am.OnSfxPlay(4);
                    Debug.Log("알 수 없는 고양이 선택됨!");
                    break;
            }
            PlayerPrefs.SetInt("selectedCatIndex", index);
            PlayerPrefs.Save();
        }
        else
        {
            // 마지막 하나가 꺼지려고 하면 다시 켬
            if (toggleGroup.ActiveToggles().Count() == 0)
            {
                changedToggle.isOn = true;
            }
        }
        
    }
    
    private void Start()
    {
        gm = GameManager.Instance;
        am = AudioManager.Instance;
        
        _constInfo = new ConstInfo();
        
        gm.constInfo =  _constInfo;
        
        registerButton.onClick.AddListener(OnRegisterButton);
        
        for (int i = 0; i < 4; i++)
        {
            RegisterSetting(i);
        }
        
        SaveCat();
        
    }

    public void SaveCat()
    {
        toggleGroup.SetAllTogglesOff(); 
        
        for (int i = 0; i < catImages.Length; i++)
        {
            // 기본값은 비활성화
            catImages[i].gameObject.SetActive(false);
            lockImages[i].gameObject.SetActive(true);
            catToggles[i].interactable = false;

            // 저장된 해금 여부 확인
            if (PlayerPrefs.GetInt("catUnlocked_" + i, 0) == 1)
            {
                catImages[i].gameObject.SetActive(true);
                lockImages[i].gameObject.SetActive(false);
                catToggles[i].interactable = true;
            }
        }
        
        SelectSavedToggleLater();
    }
    
    private void SelectSavedToggleLater()
    {
        int savedIndex = PlayerPrefs.GetInt("selectedCatIndex", -1);

        if (savedIndex >= 0 && savedIndex < catToggles.Length && catToggles[savedIndex].interactable)
        {
            catToggles[savedIndex].isOn = true;
            _saveToggle = catToggles[savedIndex];
        }
    }


    private void OnRegisterButton()
    {
        switch (couponNumber.text)
        {
            case "browncat":
                RegisterSetting(0);
                PlayerPrefs.SetInt("catUnlocked_" + 0, 1);
                break;
            case "blackcat":
                RegisterSetting(1);
                PlayerPrefs.SetInt("catUnlocked_" + 1, 1);
                break;
            case "graycat":
                RegisterSetting(2);
                PlayerPrefs.SetInt("catUnlocked_" + 2, 1);
                break;
            case "sharmcat":
                RegisterSetting(3);
                PlayerPrefs.SetInt("catUnlocked_" + 3, 1);
                break;
            
        }
        PlayerPrefs.Save();
        
        couponNumber.text = "";
    }


    private void RegisterSetting(int i)
    {
        catImages[i].gameObject.SetActive(true);
        lockImages[i].gameObject.SetActive(false);
        catToggles[i].interactable = true;
        catToggles[i].isOn = true;

    }


   
}
