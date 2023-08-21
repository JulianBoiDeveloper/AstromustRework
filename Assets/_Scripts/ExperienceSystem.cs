using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceSystem : MonoBehaviour
{
    private static ExperienceSystem instance;

    // Level and experience variables
    private int level = 1;
    private int experience = 0;
    private int experienceToNextLevel = 100;

    [SerializeField] Image expBar;
    [SerializeField] TMP_Text levelText;

    // Singleton instance
    public static ExperienceSystem Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        // Ensure only one instance of the experience system exists
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        levelText.text = level.ToString();
        expBar.fillAmount = (float)experience / (float)experienceToNextLevel;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) GiveExperience(3, 8);
    }

    // Function to give experience within a range
    public void GiveExperience(int minExperience, int maxExperience)
    {
        int experienceGained = Random.Range(minExperience, maxExperience + 1);
        experience += experienceGained;
        Debug.Log("Gained " + experienceGained + " experience!");

        // Check if level up
        if (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
        else
        {
            expBar.fillAmount = (float)experience / (float)experienceToNextLevel;
        }
    }

    // Function to level up
    private void LevelUp()
    {
        level++;
        experience -= experienceToNextLevel;
        experienceToNextLevel = CalculateNextLevelExperience();
        levelText.text = level.ToString();
        expBar.fillAmount = (float)experience / (float)experienceToNextLevel;

        Debug.Log("Leveled up! Current Level: " + level);
    }

    // Function to calculate the experience required for the next level
    private int CalculateNextLevelExperience()
    {
        return Mathf.RoundToInt(experienceToNextLevel * 1.06f); // Adjust the progression formula as desired
    }
}