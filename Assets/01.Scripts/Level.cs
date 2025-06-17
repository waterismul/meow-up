using UnityEngine;

public class Level
{
    private float[] swappingDur;

    private float currentSwappingDur;
    public float CurrentSwappingDur => currentSwappingDur;

    public void Init(int level)
    {
        swappingDur = new float[7] { 1.5f, 1.3f, 1.1f, 0.9f, 0.7f, 0.5f, 0.3f };
        currentSwappingDur = swappingDur[level];
    }

    public int LevelStep(int catCount)
    {
        int level = Mathf.Clamp(catCount / 10, 0, 6);
        return level;
    }
}
    