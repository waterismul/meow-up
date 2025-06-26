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
    
    public void FeverTimeGetItem()
    {
        gm = GameManager.Instance;
        am = AudioManager.Instance;
        
        am.OnItemPlay(0);
        gm.score += pointBonus;
        gm.scoreText.text = "SCORE : "+gm.score;
        
        transform.localScale = Vector3.zero;

    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
    }
}


