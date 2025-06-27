using UnityEngine;

public class ConstInfo
{
    private float[] swappingDur;

    private float currentSwappingDur;
    public float CurrentSwappingDur => currentSwappingDur;


    private int[] catsIndex;
    private int currentCatIndex;

    public int CurrentCatIndex
    {
        get =>  currentCatIndex;
        set=> currentCatIndex = value;
    }

    public void LevelInit(int level)
    {
        swappingDur = new float[10] { 1.3f, 1.2f, 1.1f, 1f, 0.9f, 0.8f, 0.7f , 0.7f, 0.7f, 0.7f};
        currentSwappingDur = swappingDur[level];
    }
    
    public int LevelStep(int catCount)
    {
        int level = Mathf.Clamp(catCount, 0, 9);
        return level;
    }
    
    public void CatIndexInit(int index)
    {
        catsIndex = new int[4] {0,1,2,3};
        currentCatIndex = catsIndex[index];
    }
}
    