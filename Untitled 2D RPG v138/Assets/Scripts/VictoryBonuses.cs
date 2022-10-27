using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryBonuses : MonoBehaviour
{
    [Header("Bonus Level Thresholds")]
    [SerializeField] float goldThreshold = 0.6f;
    [SerializeField] float silverThreshold = 0.75f;
    float bronzeThreshold = 1.00f;

    [Header("Base Bonus Levels")]
    [SerializeField] float baseXPBonus = 500f;
    [SerializeField] float baseMoneyBonus = 500f;

    [Header("Bonus Multipliers")]
    [SerializeField] float goldMultiplier = 2f;
    [SerializeField] float silverMultiplier = 1.5f;

    // String that indicates the bonus level
    string bonusLevel;

    // Float that gives the bonus XP and money to be had
    float bonusXPamount;
    float bonusMoneyAmount;

    // For the tooltip - indicates time it took to do the dungeon
    string timePercentage;

    // GameObject references
    GameObject hud;
    GameObject timeBonus;

    // Start is called before the first frame update
    void Start()
    {
        hud = GameObject.FindWithTag("HUD");
        timeBonus = hud.GetComponent<FindTimeBonus>().GetTimeBonusGameObject(); // Have to do it this way
    }

    // Private Functions

    // Function that sets the victory panel's bonus texts
    private void SetVictoryBonusTexts(string level)
    {
        // Set the time bonus image
        hud.GetComponent<HUD>().SetTimeBonusImageSprite(level);

        // Set the time bonus text - capitalizes first letter
        hud.GetComponent<HUD>().SetTimeBonusLevelText(char.ToUpper(level[0]) + level.Substring(1));
    }

    // Public Functions

    // Function that determines the bonus level of the dungeon
    public void DetermineBonusLevel(float maxTime, float timeRemaining)
    {
        // Create a time ratio to determine the efficiency of the run - lower is better
        float timeRatio = (maxTime - timeRemaining) / maxTime;

        // Determine the bonus level based on these thresholds
        if (timeRatio <= goldThreshold)
        {
            bonusLevel = "gold";
            bonusXPamount = goldMultiplier * baseXPBonus;
            bonusMoneyAmount = goldMultiplier * baseMoneyBonus;
            timePercentage = ((int)(goldThreshold * 100)).ToString();
        }
        else if (timeRatio <= silverThreshold)
        {
            bonusLevel = "silver";
            bonusXPamount = silverMultiplier * baseXPBonus;
            bonusMoneyAmount = silverMultiplier * baseMoneyBonus;
            timePercentage = ((int)(silverThreshold * 100)).ToString();
        }
        else
        {
            bonusLevel = "bronze";
            bonusXPamount = baseXPBonus;
            bonusMoneyAmount = baseMoneyBonus;
            timePercentage = ((int)(bronzeThreshold * 100)).ToString();
        }

        // Set the victory panel texts based on this level
        SetVictoryBonusTexts(bonusLevel);

        // Set the tooltip based on this level
        timeBonus.GetComponentInChildren<TooltipTrigger>().SetHeader(char.ToUpper(bonusLevel[0]) + bonusLevel.Substring(1) + " Bonus");
        timeBonus.GetComponentInChildren<TooltipTrigger>().SetContent("Finished the dungeon in " + timePercentage + "% of max time." + System.Environment.NewLine + "  ");
    }

    // Function that gives the bonus XP amount
    public float GetBonusXPAmount()
    {
        return bonusXPamount;
    }

    // Function that gives the bonus money amount
    public float GetBonusMoneyAmount()
    {
        return bonusMoneyAmount;
    }
}
