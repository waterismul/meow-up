using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _itemSource;
    [SerializeField] private AudioSource _bgmSource;
    
    
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Button _saveButton;
    
    [SerializeField] private AudioClip[] _sfxClips;
    [SerializeField] private AudioClip[] _itemClips;
    [SerializeField] private AudioClip[] _bgmClips;
    
    private const string BGM_KEY = "bgm";
    private const string SFX_KEY = "sfx";

    private void Start()
    {
        float bgmValue = PlayerPrefs.GetFloat(BGM_KEY, 0.5f);
        float sfxValue = PlayerPrefs.GetFloat(SFX_KEY, 0.5f);
        _bgmSlider.value = bgmValue;
        _sfxSlider.value = sfxValue;
        
        SetBGMVolume(_bgmSlider.value);
        SetSFXVolume(_sfxSlider.value);
        
        _bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        _saveButton.onClick.AddListener(SaveVolumes);
    }
    void SaveVolumes()
    {
        PlayerPrefs.SetFloat(BGM_KEY, _bgmSlider.value);
        PlayerPrefs.SetFloat(SFX_KEY, _sfxSlider.value);
        PlayerPrefs.Save(); // 저장 강제 적용
        Debug.Log("볼륨 설정 저장됨");
    }
    
    void SetBGMVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        _audioMixer.SetFloat("bgm", dB);
    }

    void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        _audioMixer.SetFloat("sfx", dB);
    }

    public void OnSfxPlay(int index)
    {
        if (index >= 0 && index < _sfxClips.Length)
        {
            _sfxSource.clip = _sfxClips[index];
            _sfxSource.Play();
        }
    }

    public void OnBgmPlay(int index)
    {
        if (index >= 0 && index < _sfxClips.Length)
        {
            _bgmSource.clip = _bgmClips[index];
            _bgmSource.Play();
        }
    }
    
    
    public void OnItemPlay(int index)
    {
        if (index >= 0 && index < _sfxClips.Length)
        {
            _itemSource.clip = _itemClips[index];
            _itemSource.Play();
        }
    }
}
