using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    //cat
    public Queue<GameObject> catPrefabObjQueue;
    public int catPrefabObjCount=20;
    public Transform catPrefabObjParent;
    public GameObject catPrefabObj;
    
    //fish item
    public Queue<GameObject> fishPrefabObjQueue;
    public int fishPrefabObjCount=5;
    public Transform fishPrefabObjParent;
    public GameObject fishPrefabObj;
    
    //time item
    public Queue<GameObject> timePrefabObjQueue;
    public int timePrefabObjCount=5;
    public Transform timePrefabObjParent;
    public GameObject timePrefabObj;
    
    private void InitPool(Queue<GameObject> prefabObjQueue, GameObject prefabObj, int prefabObjCount, Transform prefabObjParent)
    {
        for (int i = 0; i < prefabObjCount; i++)
        {
            GameObject obj = Instantiate(prefabObj, prefabObjParent);
            obj.SetActive(false);
            prefabObjQueue.Enqueue(obj);
            
        }
    }

    public GameObject GetPrefabObj(Queue<GameObject> prefabObjQueue, GameObject prefabObj, Transform prefabObjParent)
    {
        if (prefabObjQueue.Count == 0)
        {
            GameObject newObj = Instantiate(prefabObj, prefabObjParent);
            newObj.SetActive(false);
            prefabObjQueue.Enqueue(newObj);
        }
        GameObject obj =  prefabObjQueue.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnPrefabObj(GameObject obj, Queue<GameObject> prefabObjQueue)
    {
        obj.SetActive(false);
        prefabObjQueue.Enqueue(obj);
    }

    protected override void Awake()
    {
        base.Awake();
        catPrefabObjQueue = new Queue<GameObject>();
        
        //cat
        InitPool(catPrefabObjQueue, catPrefabObj, catPrefabObjCount, catPrefabObjParent);
        
        //fish item
        //InitPool(fishPrefabObjQueue, fishPrefabObj, fishPrefabObjCount, fishPrefabObjParent);
        
        //time item
        //InitPool(timePrefabObjQueue, timePrefabObj, timePrefabObjCount, timePrefabObjParent);
    }
}
