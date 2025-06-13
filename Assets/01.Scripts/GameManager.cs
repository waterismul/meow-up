using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public List<Cat> cats;
    public int catCount;
    
    [SerializeField] private GameObject floorObj;
    [SerializeField] private float downY;
    
    private GameObject _catPrefabObj;
    private Cat _catPrefabObjScript;
    private ObjectPoolManager pool;

    private void Start()
    {
        catCount = 0;
        downY = 0.86f;

        cats = new List<Cat>();
        pool = ObjectPoolManager.Instance;

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
        
    }

    IEnumerator SpawnCat()
    {
        yield return new WaitForSeconds(0.8f);
        _catPrefabObj = pool.GetPrefabObj(pool.catPrefabObjQueue, pool.catPrefabObj, pool.catPrefabObjParent);
        _catPrefabObjScript = _catPrefabObj.GetComponent<Cat>();
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
    
}