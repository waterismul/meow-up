using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Cat catPrefab;
    private Cat _catPrefabObj;
    
    private void Start()
    {
        SpawnCat();
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

    
}
