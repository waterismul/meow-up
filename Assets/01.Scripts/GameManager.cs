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
    public List<Cat> cats;
    public int catCount;
    public Image[] life;
    public TextMeshProUGUI scoreText;
    public int score;
    public GameObject countObj;
    public TextMeshPro countText;
    public float currentTime;
    public ItemManager im;
    
    [SerializeField] private GameObject floorObj;
    [SerializeField] private GameObject catObj;
    [SerializeField] private float downY;
    [SerializeField] private Image gaugeTop;
    [SerializeField] private int maxLife=5;
    [SerializeField] private float maxTime=60;
    
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI bestText;
    
    private GameObject _catPrefabObj;
    private Cat _catPrefabObjScript;
    private ObjectPoolManager pool;
    public bool isGameOver;
    private Level _level;
    private int _currentLevel;
    private int _currentLife;

    private UIManager um;
    private AudioManager am;
    public bool resumed;

    private bool hasSpawnedThisRound;
    private int bestscore;
    
    private void Start()
    {
        pool = ObjectPoolManager.Instance;
        um = UIManager.Instance;
        am = AudioManager.Instance;
        
        Time.timeScale = 0f;
    }

    public void InitSetting()
    {
        bestscore = PlayerPrefs.GetInt("bestscore");
        catCount = 0;
        score = 0;
        scoreText.text = "SCORE : "+score;
        
        _currentLife = maxLife;
        cats = new List<Cat>();
        cats.Clear();
       
        gaugeTop.fillAmount = 1f;
        downY = 0.875f;
        countObj.SetActive(false);
        
        currentTime = 0;

        for (int i=0; i < maxLife; i++)
        {
            life[i].transform.localScale = Vector3.one;
        }
        
        _level = new Level();
        _level.Init(0);
        
        floorObj.transform.position = new Vector3(0, -4.56f, 0);
        Color c = floorObj.GetComponent<Renderer>().material.color;
        c.a = 1f;
        floorObj.GetComponent<Renderer>().material.color = c;
        
        StartCoroutine(SpawnCat());
        
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
        
        if (um.IsPaused) return;
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            if (IsPointerOverUI()) return;
            if (_catPrefabObjScript is null) return;
            if (!_catPrefabObjScript.IsJumping)
                _catPrefabObjScript.Jumping();
        }

        if(!isGameOver)
            UpdateTimeUI();
        
        
        if(_level !=null)
            LevelControll();
    }

    private void LevelControll()
    {
        int newLevel = _level.LevelStep(catCount);
        if (newLevel != _currentLevel)
        {
            _currentLevel = newLevel;
            _level.Init(_currentLevel);
        }
    }

    public IEnumerator SpawnCat()
    {
        yield return new WaitForSeconds(0.8f);
        _catPrefabObj = pool.GetPrefabObj(pool.catPrefabObjQueue, pool.catPrefabObj, pool.catPrefabObjParent);
        _catPrefabObjScript = _catPrefabObj.GetComponent<Cat>();
        _catPrefabObjScript.IsJumping = false;
        _catPrefabObjScript.Init(()=>StartCoroutine(SpawnCat()));
        _catPrefabObjScript.Swapping(_level.CurrentSwappingDur);
        
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
                im.SpawnItem(im.timeObj);
            else if(rand is 2 or 1)
                im.SpawnItem(im.pointObj);
            else if(rand is 0)
                im.SpawnItem(im.minusObj);
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
        
        if(im.timeObj.activeSelf)
            im.timeObj.transform.position -= new Vector3(0, downY, 0);
        if(im.pointObj.activeSelf)
            im.pointObj.transform.position -= new Vector3(0, downY, 0);
        if(im.minusObj.activeSelf)
            im.minusObj.transform.position -= new Vector3(0, downY, 0);
    }

    private void GameOver()
    {
        isGameOver = true;
        um.OpenOverPanel();
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
        scoreText.text = "SCORE : "+score;
            
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
        if(currentTime>=maxTime) 
            currentTime = maxTime;
        currentTime += Time.deltaTime;

        float ratio = Mathf.Clamp01(1f - (currentTime / maxTime));
        gaugeTop.fillAmount = ratio;

        if (ratio < 0.3f)
        {
            gaugeTop.GetComponent<Image>().color = new Color(1f, 0f, 0f);

        }
            
        else
        {
            
            gaugeTop.GetComponent<Image>().color = new Color(0f, 0f, 0f);
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