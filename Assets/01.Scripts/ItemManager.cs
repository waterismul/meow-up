using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public GameObject timeObj;
    public GameObject pointObj;
    public GameObject minusObj;

    [SerializeField] private float timeBonus = 10f;
    [SerializeField] private int pointBonus = 10;
    [SerializeField] private int pointMinus = 20;


    private GameManager gm;
    private AudioManager am;
    
    private void Start()
    {
        gm = GameManager.Instance;
        am = AudioManager.Instance;
        InitSetting();
    }

    public void InitSetting()
    {
        
        timeObj.SetActive(false);
        timeObj.transform.localScale = Vector3.zero;
        pointObj.SetActive(false);
        pointObj.transform.localScale = Vector3.zero;
        minusObj.SetActive(false);
        minusObj.transform.localScale = Vector3.zero;
    }
    
    public void SpawnItem(GameObject itemObj)
    {
        if (!itemObj.activeSelf)
        {
            am.OnSfxPlay(2);
            var pointX = Random.Range(-1, 2);
            if (itemObj.name == "minus item")
                pointX = 0;
            itemObj.transform.position =  new Vector3(pointX, 2f, transform.position.z);
            itemObj.SetActive(true);
            itemObj.transform.DOScale(1f, 0.5f);
        }
        
    }

    public void GetItemPoint()
    {
        am.OnItemPlay(0);
        gm.score += pointBonus;
        gm.scoreText.text = "점수 : "+gm.score;
        OffItemTime(pointObj);
    }
    
   
    
    public void GetItemTime()
    {
        am.OnItemPlay(1);
        gm.currentTime -= timeBonus;
        OffItemTime(timeObj);
    }
    
    public void GetItemMinus()
    {
        am.OnItemPlay(2);
        gm.score -= pointMinus;
        gm.scoreText.text = "점수 : "+gm.score;
        OffItemTime(minusObj);
    }

    public void OffItemTime(GameObject obj)
    {
        obj.transform.localScale = Vector3.zero;
        obj.SetActive(false);
    }
    
}
