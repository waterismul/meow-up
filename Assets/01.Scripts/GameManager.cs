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
    public Image[] life;
    public GameObject countObj;
    [SerializeField] private GameObject floorObj;
    //[SerializeField] private GameObject catObj;
    [SerializeField] private float downY;
    [SerializeField] private Image gaugeTop;
    
    public int score;
    public float currentTime;
    public bool isGameOver;
    public bool resumed;
  
    
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI bestText;
    
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
    
    
    private void Start()
    {
        _pool = ObjectPoolManager.Instance;
        _um = UIManager.Instance;
        _am = AudioManager.Instance;
        constInfo = new  ConstInfo();
        
        Time.timeScale = 0f;
    }

    public int InitSetting()
    {
        //ui init
        bestscore = PlayerPrefs.GetInt("bestscore");
        catCount = 0;
        score = 0;
        scoreText.text = "SCORE : "+score;
        _currentLife = _maxLife;
        gaugeTop.fillAmount = 1f;
        downY = 0.875f;
        countObj.SetActive(false);
        currentTime = 0;
        
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
        int selectedIndex = PlayerPrefs.GetInt("selectedCatIndex", -1);
        if (selectedIndex == -1) // 선택된 고양이 인덱스가 없어 -1이라면 return으로 빠져나옴
            return selectedIndex;
        constInfo.CatIndexInit(selectedIndex);
        Debug.Log(selectedIndex);
       
        //cat init
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
        
        
        if(constInfo !=null)
            LevelControll();
    }

    private void LevelControll()
    {
        int newLevel = constInfo.LevelStep(catCount);
        if (newLevel != _currentLevel)
        {
            _currentLevel = newLevel;
            constInfo.LevelInit(_currentLevel);
        }
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
            cat.transform.position -= new Vector3(0, downY, 0);
        }

        Color c = floorObj.GetComponent<Renderer>().material.color;
        c.a = 0;
        floorObj.GetComponent<Renderer>().material.color = c;
        
        if(_im.timeObj.activeSelf)
            _im.timeObj.transform.position -= new Vector3(0, downY, 0);
        if(_im.pointObj.activeSelf)
            _im.pointObj.transform.position -= new Vector3(0, downY, 0);
        if(_im.minusObj.activeSelf)
            _im.minusObj.transform.position -= new Vector3(0, downY, 0);
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
        score += 100;
        scoreText.text = "SCORE : "+ score;
            
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
            
            gaugeTop.GetComponent<Image>().color = new Color(135/255f, 89/255f, 172/255f, 1f);
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
    
}