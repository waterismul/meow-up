using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject floorObj;
    [SerializeField] private float _downY;
    private GameObject _catPrefabObj;
    private Cat _catPrefabObjScript;
    private ObjectPoolManager pool;
    public List<Cat> cats = new List<Cat>();
    
    private void Start()
    {
        pool = ObjectPoolManager.Instance;
        
        SpawnCat();
        
        _downY = 0.86f;
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _catPrefabObjScript.Jumpping();
        }
    }

    private void SpawnCat()
    {
        _catPrefabObj = pool.GetPrefabObj(pool.catPrefabObjQueue, pool.catPrefabObj, pool.catPrefabObjParent);
        _catPrefabObjScript = _catPrefabObj.GetComponent<Cat>();
        _catPrefabObjScript.Init(SpawnCat);
        _catPrefabObjScript.Swapping();

    }

    public void DownCats()
    {
        if (cats.Count < 5) return;
        foreach (var cat in cats)
        {
           cat.transform.position -= new Vector3(0, _downY, 0);
        }
        
        floorObj.transform.position -= new Vector3(0, _downY, 0);
    }

    
}
