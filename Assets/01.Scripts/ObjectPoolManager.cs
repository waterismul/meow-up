using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private GameObject _catPrefabObj;
    private Queue<GameObject> _catPrefabObjPool;
    private Transform _parent;

    private void InitPool()
    {
        
    }
}
