using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolStats : MonoBehaviour
{
    [System.Serializable]
    public struct LevelStats
    {
        public float miningSpeed;
    }

    public List<LevelStats> levelStats;
    [HideInInspector] public LevelStats currentStats;
    public int currentLevel = 0;
    public enum ToolType
    {
        pickaxe,
        axe,
        gun
    }

    public ToolType tooltype;

    private void Start()
    {
        if (levelStats.Count > 0)
            currentStats = levelStats[currentLevel];

        currentStats.miningSpeed = levelStats[currentLevel].miningSpeed;
    }

    public void IncreaseLevel()
    {
        if (currentLevel < levelStats.Count)
        {
            currentLevel++;
            currentStats = levelStats[currentLevel];
            GetComponent<Item1>().icon = GetComponent<Item1>().upgradeIcon[currentLevel];
        }
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
        currentStats = levelStats[currentLevel];
        GetComponent<Item1>().icon = GetComponent<Item1>().upgradeIcon[currentLevel];
    }

    public bool IsMaxLevel()
    {
        if (currentLevel >= levelStats.Count - 1) return true;
        return false;
    }
}
