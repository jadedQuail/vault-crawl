using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Master : MonoBehaviour
{
    // This is a script for an object that will exist between scenes
    // So it can hold anything that needs to survive a scene transition

    public static Master instance;

    private string entryGate;

    private bool playerRevived;
    private bool playerWon;

    private ItemIcon disabledIcon;

    [SerializeField] List<ItemCard> allItems;

    [SerializeField] string homeSceneName;

    // GameObject references
    GameObject hud;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        // Keep this object in all scenes; destroy clones
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        playerRevived = true;

        hud = GameObject.FindWithTag("HUD");
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGate();

        // If the player is out of the dungeon - either for winning or losing - revive the player
        if (!player.GetComponent<PlayerController>().GetInDungeon() && (!playerRevived || playerWon))
        {
            RevivePlayer();
            playerRevived = true;
            playerWon = false;
        }

        // If we're in a dungeon and the timer has been set, we're loaded; fade from black
        if (GameObject.FindWithTag("DungeonManager") != null && GameObject.FindWithTag("DungeonManager").GetComponent<DungeonTimer>().GetTimerSet())
        {
            if (hud.GetComponent<UIFade>().GetFadeHold())
            {
                hud.GetComponent<UIFade>().FadeFromBlack();
            }
        }

        // If the player is no longer hurt, then re-enable the icon we stored
        if (!player.GetComponent<Health>().GetIsHurt() && disabledIcon != null)
        {
            EnableDisabledIcon();
        }
    }

    // Function that checks for the existence of an entry gate and then moves the player to it
    private void CheckForGate()
    {
        // Once entryGate is not null, then a transition has occurred and the player needs to be moved to the entry gate
        if (entryGate != null)
        {
            // Find every entrance in this scene
            foreach (GameObject gate in GameObject.FindGameObjectsWithTag("AreaEntrance"))
            {
                // Find the one with the same gate name
                if (entryGate == gate.GetComponent<AreaEntrance>().GetEntryGate())
                {
                    // Transition the player
                    gate.GetComponent<AreaEntrance>().EnterPlayer();

                    // If we're in a dungeon, override EnterPlayer() and set the player's animations to be downward, on entry
                    if (GameObject.FindWithTag("DungeonManager") != null)
                    {
                        player.GetComponent<PlayerController>().SetSpawnAnimations();
                    }

                    // If we're in a dungeon, hold on black
                    if (GameObject.FindWithTag("DungeonManager") != null)
                    {
                        hud.GetComponent<UIFade>().FadeHoldBlack();
                    }
                    else  // Otherwise, load up like normal
                    {
                        hud.GetComponent<UIFade>().MakeFadeScreenBlack();
                        hud.GetComponent<UIFade>().FadeFromBlack();
                    }

                    // If this is a dungeon, do the following
                    if (GameObject.FindWithTag("DungeonManager") != null)
                    {
                        // Turn on the timer panel (which also enables an arrow count visual)
                        hud.GetComponent<HUD>().EnableTimerPanel(true);

                        // Set the player's "in dungeon" flag to true
                        player.GetComponent<PlayerController>().IsInDungeon(true);
                    }

                    // Nullify the entry gate (i.e. reset it)
                    entryGate = null;
                }
            }
        }
    }

    // PUBLIC FUNCTIONS

    // Function for setting the entry gate for a transition (function calls when transition occurs)
    public void SetEntryGate(string gate)
    {
        entryGate = gate;
    }

    // Function for getting a random item from the "all items" list
    public ItemCard GetRandomItem()
    {
        return allItems[Random.Range(0, allItems.Count)];
    }

    // Function for spawning the player back in town
    public void RespawnInTown()
    {
        // If the player won, give dungeon rewards
        if (playerWon)
        {
            // Obtain reference to VictoryBonuses in scene
            VictoryBonuses victoryBonuses = GameObject.FindWithTag("DungeonManager").GetComponent<VictoryBonuses>();

            // These are just values for the demo
            player.GetComponent<MoneyController>().AddToPlayerMoney((int)victoryBonuses.GetBonusMoneyAmount());
            player.GetComponent<PlayerLeveling>().AddXP((int)victoryBonuses.GetBonusXPAmount());
        }

        // Respawn in town
        SceneManager.LoadScene(homeSceneName);

        // Set the player's "in dungeon" flag to false
        player.GetComponent<PlayerController>().IsInDungeon(false);

        // Turn off the timer panel
        hud.GetComponent<HUD>().EnableTimerPanel(false);
    }

    // Function that revives the player and resets the game after he dies
    public void RevivePlayer()
    {
        // Close the Game Over panel or Victory panel
        hud.GetComponent<HUD>().CloseGameOverPanel();
        hud.GetComponent<HUD>().CloseVictoryPanel();

        // Reset all the player's abilities
        player.GetComponent<PlayerAbilities>().ResetAllAbilities();

        // Reset the player's "died by time" status
        player.GetComponent<Health>().SetPlayerDiedByTime(false);

        // Give the player's health back
        player.GetComponent<Health>().SetHealth(player.GetComponent<Health>().GetMaxHealth());

        // Reset the player to his town spawn position
        player.transform.position = player.GetComponent<PlayerController>().GetTownSpawnPos();

        // Set the player's status to "alive" again
        player.GetComponent<Health>().SetPlayerDead(false);

        // Enable the player's weapon
        player.GetComponent<PlayerController>().ToggleWeapon("enable");

        // Allow the player to move again
        player.GetComponent<PlayerController>().SetCanMove(true);

        // Allow the player to attack again
        player.GetComponent<PlayerController>().SetCanAttack(true);
    }

    // Function that gets whether or not the player is revived
    public bool GetPlayerRevived()
    {
        return playerRevived;
    }

    // Function that sets whether or not the player is revived
    public void SetPlayerRevived(bool value)
    {
        playerRevived = value;
    }

    // Function that sets whether or not the player is currently in a state of having just won the dungeon
    public void SetPlayerWon(bool value)
    {
        playerWon = value;
    }

    // Function that stores the disabled talisman use icon
    public void SetDisabledIcon(ItemIcon itemIcon)
    {
        disabledIcon = itemIcon;
    }

    // Function that re-enables the disabled talisman use icon
    public void EnableDisabledIcon()
    {
        disabledIcon.enabled = true;
        disabledIcon = null;
    }
}
