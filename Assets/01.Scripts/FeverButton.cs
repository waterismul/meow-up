using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FeverButton : MonoBehaviour
{
    private GameManager gm;
    private AudioManager am;
    private int pointBonus=10;
    
    public void FeverTimeGetItem()
    {
        gm = GameManager.Instance;
        am = AudioManager.Instance;
        
        am.OnItemPlay(0);
        gm.score += pointBonus;
        gm.scoreText.text = "점수 : "+gm.score;
        
        gameObject.GetComponent<Button>().interactable = false;

    }

    private void OnEnable()
    {
        gameObject.GetComponent<Button>().interactable = true;
    }
}


