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

    private Level _level;
    
    private Toggle _saveToggle;

    private GameManager gm;

    public void OnToggleChanged(Toggle changedToggle)
    {
        if (changedToggle.isOn)
        {
            // 하나만 선택되도록 하고, 마지막 하나를 끄는 걸 막음
            if (toggleGroup.ActiveToggles().Count() == 0)
            {
                changedToggle.isOn = true; // 다시 켜버림
                return;
            }

            _saveToggle = changedToggle;
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
        _level = new Level();
        
        gm._level =  _level;
        
        registerButton.onClick.AddListener(OnRegisterButton);

        SaveCat();
        
    }

    public void SaveCat()
    {
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
    }


    private void OnRegisterButton()
    {
        switch (couponNumber.text)
        {
            case "brown123":
                RegisterSetting(0);
                break;
            case "black456":
                RegisterSetting(1);
                break;
            case "gray1234":
                RegisterSetting(2);
                break;
            case "sham1234":
                RegisterSetting(3);
                break;
        }
        
        couponNumber.text = "";
    }


    private void RegisterSetting(int i)
    {
        catImages[i].gameObject.SetActive(true);
        lockImages[i].gameObject.SetActive(false);
        catToggles[i].interactable = true;

    }

    public void OnSaveButton()
    {
        switch (_saveToggle.name)
        {
            case "brown Toggle":
                _level.CatIndexInit(0);
                PlayerPrefs.SetInt("catUnlocked_" + 0, 1);
                break;
            case "black Toggle":
                _level.CatIndexInit(1);
                PlayerPrefs.SetInt("catUnlocked_" + 1, 1);
                break;
            // 필요하면 더 추가
            default:
                return;
                break;
        }

        PlayerPrefs.Save();
    }

    
}
