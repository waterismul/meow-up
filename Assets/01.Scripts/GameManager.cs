using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Cat catPrefab;
    [SerializeField] private GameObject floorObj;
    [SerializeField] private float _downY;
    private Cat _catPrefabObj;
    public List<Cat> cats;
    
    private void Start()
    {
        cats = new List<Cat>();
        
        SpawnCat();
        
        _downY = 0.86f;
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _catPrefabObj.Jumpping();
        }
    }

    private void SpawnCat()
    {
        _catPrefabObj = Instantiate(catPrefab);
        
        _catPrefabObj.Init(SpawnCat);
        _catPrefabObj.Swapping();

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
