using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public List<Cat> cats;
    public int catCount;
    public Image[] life;
    
    [SerializeField] private GameObject floorObj;
    [SerializeField] private float downY;
    [SerializeField] private Image gaugeTop;
    [SerializeField] private int maxLife=5;
    [SerializeField] private float maxTime=3000f;
    
    private GameObject _catPrefabObj;
    private Cat _catPrefabObjScript;
    private ObjectPoolManager pool;
    private int _currentLife;
    private float _currentTime;
    private bool _isGameOver;
   
    
    private void Start()
    {
        catCount = 0;
        downY = 0.86f;

        cats = new List<Cat>();
        pool = ObjectPoolManager.Instance;

        _currentLife = maxLife;

        gaugeTop.fillAmount = 1f;
        
        
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
    }
    
    public IEnumerator DownCats()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Cat cat in cats)
        {
            cat.transform.position -= new Vector3(0, downY, 0);
        }

        floorObj.transform.position -= new Vector3(0, downY, 0);
    }

    private void UpdateLifeUI()
    {
        Color c = life[_currentLife].color;
        c.a = 0;
        life[_currentLife].color = c;
    }

    private void GameOver()
    {
        _isGameOver = true;
        Time.timeScale = 0;
        Debug.Log("Game Over");
    }
    
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
        _currentTime += Time.deltaTime;

        float ratio = Mathf.Clamp01(1f - (_currentTime / maxTime));
        gaugeTop.fillAmount = ratio;

        if (ratio <= 0f)
        {
            GameOver();
        }

    }
    
}