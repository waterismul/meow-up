using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FeverButton : MonoBehaviour
{
    private GameManager gm;
    private AudioManager am;
    private int pointBonus=100;
    [SerializeField] private TextMeshProUGUI pointText;
    
    public void FeverTimeGetItem()
    {
        gm = GameManager.Instance;
        am = AudioManager.Instance;
        
        am.OnItemPlay(0);
        gm.score += pointBonus;
        gm.scoreText.text = "점수 : "+gm.score;
        
        pointText.gameObject.SetActive(true);
        gameObject.GetComponent<Button>().interactable = false;

    }

    private void OnEnable()
    {
        pointText.gameObject.SetActive(false);
        gameObject.GetComponent<Button>().interactable = true;
    }
}


