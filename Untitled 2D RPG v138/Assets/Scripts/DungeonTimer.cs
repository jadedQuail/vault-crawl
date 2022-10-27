using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTimer : MonoBehaviour
{
    private float totalTime;
    private float timer;

    [SerializeField] float timePerEnemy;
    [SerializeField] float timePerBaron;
    [SerializeField] float warningPercentage;

    private string timerString;

    private bool warningDisplayed;

    private bool timerSet = false;

    // A boolean that controls when the timer can begin (when player takes first step)
    private bool timerBegin = false;

    private int enemyCount;
    private int baronCount;
    bool baronCountSet = false;

    GameObject hud;
    GameObject gameController;
    GameObject player;
    GameObject master;

    BackgroundGenerator bgGenerator;

    private void Start()
    {
        // Initialize references
        hud = GameObject.FindWithTag("HUD");
        gameController = GameObject.FindWithTag("GameController");
        player = GameObject.FindWithTag("Player");
        master = GameObject.FindWithTag("Master");

        bgGenerator = GameObject.FindWithTag("DungeonManager").GetComponent<BackgroundGenerator>();

        // Set so warning does not display
        warningDisplayed = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Always update the number of enemies
        enemyCount = gameController.GetComponent<GameController>().GetEnemyList_NoBounties().Count;

        hud.GetComponent<HUD>().SetEnemyCountText(enemyCount);
        if (baronCountSet) { hud.GetComponent<HUD>().SetBaronCountText(baronCount); }

        // Set the timer once the background is generated and the player is not dead
        if (bgGenerator.GetBackgroundGenerated() && !player.GetComponent<Health>().GetPlayerDead() && !timerSet)
        {
            // Baron loading
            GetComponent<BaronLoader>().FindUnusedSpawnNodes();
            baronCount = GetComponent<BaronLoader>().LoadBarons();
            baronCountSet = true;

            // Set timer w/ pre-set time per enemy
            totalTime = (enemyCount * timePerEnemy) + (baronCount * timePerBaron);
            hud.GetComponent<HUD>().SetEnemyCountText(enemyCount);

            timer = totalTime;
            timerSet = true;
        }

        if (timerSet && player.GetComponent<PlayerController>().GetIsMoving()) { timerBegin = true; }

        if (timerBegin)
        {
            if (timer > 0 && !hud.GetComponent<HUD>().AnyMenuOpen()) { timer -= Time.deltaTime; }
        }

        // Calculate minutes
        string minutes;
        if (Mathf.FloorToInt(timer/60) < 10)
        {
            minutes = "0" + Mathf.FloorToInt(timer / 60).ToString();
        }
        else
        {
            minutes = Mathf.FloorToInt(timer / 60).ToString();
        }

        // Calculate seconds
        string seconds;
        if (Mathf.FloorToInt(timer % 60) < 10)
        {
            seconds = "0" + Mathf.FloorToInt(timer % 60).ToString();
        }
        else
        {
            seconds = Mathf.FloorToInt(timer % 60).ToString();
        }

        // Set the timer text
        if (!timerSet) { timerString = "--:--"; }
        else if (timer >= 0) { timerString = minutes + ":" + seconds; }
        else { timerString = "00:00"; }

        // Display a warning if time is running out
        if (timer <= (totalTime * warningPercentage) && warningDisplayed == false && bgGenerator.GetBackgroundGenerated())
        {
            hud.GetComponent<HUD>().EnableWarningPanel(true);
            hud.GetComponent<HUD>().SetWarningText("Time is running out!");

            warningDisplayed = true;
        }

        // If time runs out, kill the player.
        if (timer <= 0 && timerSet)
        {
            player.GetComponent<Health>().SetPlayerDiedByTime(true);
        }

        // If the player killed all the enemies and barons, they beat the dungeon; start victory process
        if ((enemyCount + baronCount) <= 0 && timerSet)
        {
            // Determine the level of bonus that the player got
            GetComponent<VictoryBonuses>().DetermineBonusLevel(totalTime, timer);

            // Set the money and XP bonus texts to reflect this bonus level determined above
            hud.GetComponent<HUD>().SetCoinBonusAmountText(GetComponent<VictoryBonuses>().GetBonusMoneyAmount().ToString());
            hud.GetComponent<HUD>().SetXPBonusAmountText(GetComponent<VictoryBonuses>().GetBonusXPAmount().ToString());

            // Open the victory panel
            hud.GetComponent<HUD>().OpenVictoryPanel();

            // Indicate that the player is in a state of having won the dungeon
            master.GetComponent<Master>().SetPlayerWon(true);
        }
    }

    // PUBLIC FUNCTIONS

    // Function that gets the status of the timer
    public bool GetTimerSet()
    {
        return timerSet;
    }

    // Function that sets the status of the timer
    public void SetTimerSet(bool value)
    {
        timerSet = value;
    }

    // Function that returns the timer string for the dungeon
    public string GetTimerString()
    {
        return timerString;
    }

    // Function that sets timer begin boolean
    public void SetTimerBegin(bool value)
    {
        timerBegin = value;
    }

    // Function that updates the baron count
    public void DecrementBaronCount()
    {
        baronCount--;
    }
}
