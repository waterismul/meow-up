using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    //cat
    public Queue<GameObject> catPrefabObjQueue;
    public int catPrefabObjCount=20;
    public Transform catPrefabObjParent;
    public GameObject catPrefabObj;
    private List<GameObject> catList = new List<GameObject>();
    
    private void InitPool(Queue<GameObject> prefabObjQueue, GameObject prefabObj, int prefabObjCount, Transform prefabObjParent)
    {
        for (int i = 0; i < prefabObjCount; i++)
        {
            GameObject obj = Instantiate(prefabObj, prefabObjParent);
            obj.SetActive(false);
            prefabObjQueue.Enqueue(obj);
            catList.Add(obj);
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
        
        Collider2D coll= obj.GetComponent<Collider2D>();
        coll.enabled = true;
        
        Rigidbody2D rb= obj.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        
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
        
    }
    
    public void ResetPool()
    {
        foreach (var obj in catList)
        {
            if (obj != null)
            {
               Destroy(obj);
            }
        }
        
        catPrefabObjQueue.Clear();

        InitPool(catPrefabObjQueue, catPrefabObj, catPrefabObjCount,catPrefabObjParent);
    }


}
