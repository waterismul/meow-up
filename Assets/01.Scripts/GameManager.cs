using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using TMPro;
using UnityEngine;
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
    
    [SerializeField] private GameObject floorObj;
    [SerializeField] private GameObject catObj;
    [SerializeField] private float downY;
    [SerializeField] private Image gaugeTop;
    [SerializeField] private int maxLife=5;
    [SerializeField] private float maxTime=3000f;
    
    
    private GameObject _catPrefabObj;
    private Cat _catPrefabObjScript;
    private ObjectPoolManager pool;
    private int _currentLife;
    public float currentTime;
    private bool _isGameOver;
    private BoxCollider2D _catRb;
    
    public ItemManager im;
    
    
    private void Start()
    {
        catCount = 0;
        _currentLife = maxLife;
        cats = new List<Cat>();
        pool = ObjectPoolManager.Instance;
        gaugeTop.fillAmount = 1f;
        downY = 0.875f;
        countObj.SetActive(false);
        
        StartCoroutine(SpawnCat());
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_catPrefabObjScript is null) return;
            if (!_catPrefabObjScript.IsJumping)
                _catPrefabObjScript.Jumping();
        }

        if(!_isGameOver)
            UpdateTimeUI();
    }

    IEnumerator SpawnCat()
    {
        yield return new WaitForSeconds(0.8f);
        _catPrefabObj = pool.GetPrefabObj(pool.catPrefabObjQueue, pool.catPrefabObj, pool.catPrefabObjParent);
        _catPrefabObjScript = _catPrefabObj.GetComponent<Cat>();
        _catPrefabObjScript.IsJumping = false;
        _catPrefabObjScript.Init(()=>StartCoroutine(SpawnCat()));
        _catPrefabObjScript.Swapping();
        
        if(cats.Count>4 && Random.Range(0,10) <= 1)
            if(Random.Range(0,2) == 1)
                im.SpawnItemTime();
            else
                im.SpawnItemPoint();
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
    }

    private void GameOver()
    {
        _isGameOver = true;
        Time.timeScale = 0;
        Debug.Log("Game Over");
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
        DOVirtual.DelayedCall(0.5f, () => { countObj.SetActive(false); });
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
        
        Debug.Log("currentLife : "+_currentLife);
    }
    
    
    private void UpdateTimeUI()
    {
        if(currentTime>maxTime) 
            currentTime = maxTime;
        currentTime += Time.deltaTime;

        float ratio = Mathf.Clamp01(1f - (currentTime / maxTime));
        gaugeTop.fillAmount = ratio;
        
        if(ratio<0.3f)
            gaugeTop.GetComponent<Image>().color = new Color(1f, 0f, 0f);
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
        Color c = life[_currentLife].color;
        c.a = 0;
        life[_currentLife].color = c;
    }
    
}