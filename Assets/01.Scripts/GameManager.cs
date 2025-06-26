using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    
    [Header("[Cat]")]
    public List<Cat> cats;
    public int catCount;
    
    [Header("[GameUI]")]
    public TextMeshProUGUI scoreText;
    public TextMeshPro countText;
    public TextMeshPro comboText;
    public Image[] life;
    public GameObject countObj;
    [SerializeField] private GameObject floorObj;
    [SerializeField] private float downY;
    [SerializeField] private Image gaugeTop;
    [SerializeField] private GameObject pauseButton;
    private VertexGradient gradient;
    
    public int score;
    public float currentTime;
    public bool isGameOver;
    public bool resumed;
  
    
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI bestText;
    public TextMeshProUGUI feverTimeTitle;
    
    //game
    private int _maxLife=5;
    private float _maxTime=60;
    private GameObject _catPrefabObj;
    private Cat _catPrefabObjScript;
    private int _currentLevel;
    private int _currentLife;
    
    public ConstInfo constInfo;
    private ObjectPoolManager _pool;
    private UIManager _um;
    private AudioManager _am;
    public ItemManager _im;

    private bool hasSpawnedThisRound;
    private int bestscore;
    public int comboCount;
    
    private void Start()
    {
        _pool = ObjectPoolManager.Instance;
        _um = UIManager.Instance;
        _am = AudioManager.Instance;
        constInfo = new ConstInfo();
        
        Time.timeScale = 0f;
        _am.OnBgmPlay(0);
    }

    public int InitSetting()
    {
        //ui init
        bestscore = PlayerPrefs.GetInt("bestscore");
        catCount = 0;
        score = 0;
        currentTime = 0;
        Gradient();
        
        //game init
        scoreText.text = "점수 : "+score;
        _currentLife = _maxLife;
        gaugeTop.fillAmount = 1f;
        downY = 0.875f;
        countObj.SetActive(false);
        
        
        for (int i=0; i < _maxLife; i++)
        {
            life[i].transform.localScale = Vector3.one;
        }
        
        floorObj.transform.position = new Vector3(0, -4.56f, 0);
        Color c = floorObj.GetComponent<Renderer>().material.color;
        c.a = 1f;
        floorObj.GetComponent<Renderer>().material.color = c;
        
        //level init
        constInfo.LevelInit(0);
        comboCount = 0;
        
        //cat init
        int selectedIndex = PlayerPrefs.GetInt("selectedCatIndex", -1);
        if (selectedIndex == -1) // 선택된 고양이 인덱스가 없어 -1이라면 return으로 빠져나옴
            return selectedIndex;
        constInfo.CatIndexInit(selectedIndex);
        Debug.Log(selectedIndex);
        
        cats = new List<Cat>();
        cats.Clear();
        _pool.SpawnCatSetting(selectedIndex);
        StartCoroutine(SpawnCat());
        
        return selectedIndex; 
    }
    
    bool IsPointerOverUI()
    {
#if UNITY_EDITOR
        return EventSystem.current.IsPointerOverGameObject(); // 마우스용
#else
    if (Input.touchCount > 0)
        return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId); // 터치용
    return false;
#endif
    }

    
    private void Update()
    {
        if (resumed)
        {
            if(!Input.GetMouseButton(0))
                resumed = false;
        }
        
        if (_um.IsPaused) return;
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            if (IsPointerOverUI()) return;
            if (_catPrefabObjScript is null) return;
            if (!_catPrefabObjScript.IsJumping)
                _catPrefabObjScript.Jumping();
        }

        if(!isGameOver)
            UpdateTimeUI();
            
    }
    

    public IEnumerator SpawnCat()
    {
        yield return new WaitForSeconds(0.8f);
        _catPrefabObj = _pool.GetPrefabObj(_pool.catPrefabObjQueue, _pool.catPrefabObj[constInfo.CurrentCatIndex], _pool.catPrefabObjParent);
        _catPrefabObjScript = _catPrefabObj.GetComponent<Cat>();
        _catPrefabObjScript.IsJumping = false;
        _catPrefabObjScript.Init(()=>StartCoroutine(SpawnCat()));
        _catPrefabObjScript.Swapping(constInfo.CurrentSwappingDur);
        
        SpawnItem();
        
    }

    private void SpawnItem()
    {
        if (catCount == 0) return;
        if (catCount % 4 == 0 && !hasSpawnedThisRound)
        {
            hasSpawnedThisRound = true;
            int rand = Random.Range(0, 6);
            if(rand >= 3)
                _im.SpawnItem(_im.timeObj);
            else if(rand is 2 or 1)
                _im.SpawnItem(_im.pointObj);
            else if(rand is 0)
                _im.SpawnItem(_im.minusObj);
        }
        else
        {
            hasSpawnedThisRound = false;
        }
       
    }
    
    
    public IEnumerator DownCats()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Cat cat in cats)
        {
            cat.transform.DOMoveY(cat.transform.position.y-downY, 0.5f);
            //cat.transform.position -= new Vector3(0, downY, 0);
        }

        Color c = floorObj.GetComponent<Renderer>().material.color;
        if(c.a!=0)
            floorObj.transform.DOMoveY(floorObj.transform.position.y-downY, 0.5f).OnComplete(() =>
            {
                Color c = floorObj.GetComponent<Renderer>().material.color;
                c.a = 0;
                floorObj.GetComponent<Renderer>().material.color = c;
            });
        
        if(_im.timeObj.activeSelf)
            _im.timeObj.transform.DOMoveY(_im.timeObj.transform.position.y-downY, 0.5f);
        if(_im.pointObj.activeSelf)
            _im.pointObj.transform.DOMoveY(_im.pointObj.transform.position.y-downY, 0.5f);
        if(_im.minusObj.activeSelf)
            _im.minusObj.transform.DOMoveY(_im.minusObj.transform.position.y-downY, 0.5f);
    }

    private void GameOver()
    {
        isGameOver = true;
        _um.OpenOverPanel();
        isGameOver = false;
        resultText.text = score.ToString();
        
        var result = Mathf.Max(bestscore, score);
        PlayerPrefs.SetInt("bestscore", result);
        PlayerPrefs.Save();
        bestText.text = PlayerPrefs.GetInt("bestscore").ToString();
        
    }

    public void CountCat(GameObject currentCat)
    {
        cats.Add(currentCat.GetComponent<Cat>());
        catCount += 1;
        
        countObj.transform.position = currentCat.transform.position + new Vector3(1f, 1f, transform.position.z);
        countText.text = catCount.ToString();
        countObj.SetActive(true);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            countObj.SetActive(false);
        });
    }
    
    //UI
    public void DecreaseLife()
    {
        if (_currentLife <= 0)
        {
            GameOver();
            return;
        }
        _currentLife--;
        UpdateLifeUI();
    }
    
    
    private void UpdateTimeUI()
    {
        if(currentTime>=_maxTime) 
            currentTime = _maxTime;
        currentTime += Time.deltaTime;

        float ratio = Mathf.Clamp01(1f - (currentTime / _maxTime));
        gaugeTop.fillAmount = ratio;

        if (ratio < 0.3f)
        {
            gaugeTop.GetComponent<Image>().color = new Color(1f, 0f, 0f,1f);

        }
            
        else
        {
            
            gaugeTop.GetComponent<Image>().color = new Color32(135, 89, 172, 255);
        }

        if (ratio <= 0f)
        {
            GameOver();
        }

    }
    
    private void UpdateLifeUI()
    {
        life[_currentLife].transform.DOScale(0f, 0.3f);
    }


    public void ComboInit()
    {
        comboCount++;
        comboCount = constInfo.LevelStep(comboCount);
        constInfo.LevelInit(comboCount);

        switch (comboCount)
        {
            case 0:
                comboText.gameObject.SetActive(false);
                break;
            case 1:
                comboText.gameObject.SetActive(false);
                break;
            case 2:
                comboText.color = new Color32(200, 96, 106, 255);
                comboText.enableVertexGradient = false;
                break;
            case 3:
                comboText.color = new Color32(255, 153, 17, 255);
                comboText.enableVertexGradient = false;
                break;
            case 4:
                comboText.color = new Color32(90, 172, 108, 255);
                comboText.enableVertexGradient = false;
                break;
            case 5:
                comboText.color = new Color32(90, 172, 255, 255);
                comboText.enableVertexGradient = false;
                break;
            case 6:
                comboText.color = new Color32(134, 89, 171, 255);
                comboText.enableVertexGradient = false;
                break;
            case 7:
                comboText.color = Color.white;
                comboText.colorGradient = gradient;
                comboText.enableVertexGradient = true;
                ComboReset();
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    
                    OnFeverTime();

                });
                break;
        }

        if (comboCount > 1 && comboCount<7)
        {
            comboText.text = $"Combo{comboCount}";
            comboText.gameObject.SetActive(true);
        }
        else if(comboCount is 7)
            comboText.text = "Combo Max";
        score += 100+100*(comboCount-1);
        scoreText.text = "점수 : "+ score;
    }

    public void ComboReset()
    {
        constInfo.LevelInit(0);
        comboCount = 0;
    }

    public void Gradient()
    {
        // Gradient 생성
        gradient = new VertexGradient(
            new Color32(200, 75, 75, 255),    // topLeft (밝은 빨강)
            new Color32(255, 212, 9, 255),   // topRight (밝은 주황)
            new Color32(90, 170, 108, 255),  // bottomLeft (하늘색)
            new Color32(90, 172, 255, 255) 
        );
    }

    private void OnFeverTime()
    {
        feverTimeTitle.colorGradient = gradient;
        _um.OpenFeverPanel();
        Time.timeScale = 0f;
        _am.OnBgmPlay(1);
        pauseButton.gameObject.SetActive(false);
        
        DOVirtual.DelayedCall(5f,()=>
        {
            _um.CloseFeverPanel();
            Time.timeScale = 1f;
            _am.OnBgmPlay(0);
            pauseButton.gameObject.SetActive(true);
        });

    }
    
}