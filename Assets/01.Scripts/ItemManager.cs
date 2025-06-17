using DG.Tweening;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject timeObj;
    public GameObject pointObj;
    
    [SerializeField] private float timeBonus=10f;
    [SerializeField] private int pointBonus=10;

    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;
        timeObj.SetActive(false);
        timeObj.transform.localScale = Vector3.zero;
        pointObj.SetActive(false);
        pointObj.transform.localScale = Vector3.zero;
    }
    
    public void SpawnItemTime()
    {
        if (!timeObj.activeSelf)
        {
            var pointX = Random.Range(-1, 2);
            timeObj.transform.position =  new Vector3(pointX, 2f, transform.position.z);
            timeObj.SetActive(true);
            timeObj.transform.DOScale(1f, 0.5f);
        }
        
    }

    public void SpawnItemPoint()
    {
        if (!pointObj.activeSelf)
        {
            var pointX = Random.Range(-1, 2);
            pointObj.transform.position =  new Vector3(pointX, 2f, transform.position.z);
            pointObj.SetActive(true);
            pointObj.transform.DOScale(1f, 0.5f);
        }
    }

    public void GetItemPoint()
    {
        gm.score += pointBonus;
        gm.scoreText.text = "SCORE : "+gm.score;
        OffItemTime(pointObj);
    }
    
    public void GetItemTime()
    {
        gm.currentTime -= timeBonus;
        OffItemTime(timeObj);
    }

    public void OffItemTime(GameObject obj)
    {
        obj.transform.localScale = Vector3.zero;
        obj.SetActive(false);
    }
}
