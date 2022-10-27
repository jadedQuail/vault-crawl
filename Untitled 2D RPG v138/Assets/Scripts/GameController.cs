using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private GameObject[] enemies;

    private GameObject player;
    private GameObject hud;

    private bool canOpenMenus = true;
    private bool canOpenInventory = true;

    // A variable that determines whether or not counters can countdown (i.e. cooldown counters)
    private bool canCountdown = true;

    private void Start()
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

        // Because Unity messes this up sometimes
        canOpenMenus = true;
        canOpenInventory = true;
        canCountdown = true;

        player = GameObject.FindWithTag("Player");
        hud = GameObject.FindWithTag("HUD");
    }

    private void Update()
    {
        if ((hud.GetComponent<HUD>().AnyMenuOpen() && !hud.GetComponent<HUD>().GetVictoryPanelOpen()) || player.GetComponent<PlayerController>().GetInDialogue())
        {
            SetCanCountdown(false);
        }
        else
        {
            SetCanCountdown(true);
        }
    }

    // PUBLIC FUNCTIONS

    // Function for finding every enemy in the scene
    public void FindEnemies()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy") != null) 
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }
    }

    // Function for finding every NPC in the scene
    public GameObject[] GetAllNPCs()
    {
        return GameObject.FindGameObjectsWithTag("NPC");
    }

    // Function for getting the enemy list
    public GameObject[] GetEnemyList()
    {
        return enemies;
    }

    // Function for getting the enemy list, minus bounty enemies (for dungeon counts)
    public List<GameObject> GetEnemyList_NoBounties()
    {
        List<GameObject> enemies_noBounties = new List<GameObject>();
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyController>().GetEnemyType() != "bounty" && !enemy.GetComponent<EnemyController>().GetIsBaron())
            {
                enemies_noBounties.Add(enemy);
            }
        }
        return enemies_noBounties;
    }

    // Function for nullifying the enemy at a particular index value
    public void NullifyEnemyAtIndex(int index)
    {
        if (index <= enemies.Length && index != -1) // 'System.Array.IndexOf' sometimes returns -1, this should be disregarded
        {
            if (enemies[index] != null)
            {
                enemies[index] = null;
            }
        }
    }

    // Function for freezing or unfreezing all enemies during pauses
    public void FreezeEnemies(bool value)
    {
        // Find enemies again
        FindEnemies();

        if (enemies != null)
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                // We're freezing, or the enemy is not frozen by an ability of the player
                if (value == true || !enemy.GetComponent<EnemyController>().GetFrozenByAbility())
                {
                    enemy.GetComponent<EnemyController>().SetCanMove(!value);
                    enemy.GetComponent<EnemyController>().SetCanAttack(!value);
                    if (enemy.GetComponent<EnemyController>().GetIsBaron())
                    {
                        enemy.GetComponentInParent<BaronAnimations>().ToggleAnimatorPause(value);
                    }
                    else
                    {
                        enemy.GetComponent<EnemyController>().ToggleAnimatorPause(value);
                    }    
                }
            }
        }
    }

    // Function for freezing all elements of the game
    public void FreezeMovingObjects()
    {
        // Freeze the player
        player.GetComponent<PlayerController>().SetCanMove(false);
        player.GetComponent<PlayerController>().SetCanAttack(false);
        player.GetComponent<PlayerController>().StopVelocity();
        player.GetComponent<PlayerController>().ToggleAnimatorPause(true);

        // Freeze cooldowns and players' abilities
        player.GetComponent<PlayerAbilities>().FreezeAbilities();

        // Freeze any trails, if they exist
        foreach (GameObject trail in GameObject.FindGameObjectsWithTag("Trail"))
        {
            trail.GetComponent<Trail>().ToggleAnimatorPause(true);
        }

        // Freeze the player's arrows
        FreezeAllArrows();

        // Freeze all enemies
        FreezeEnemies(true);

        // Freeze all NPCs
        FreezeNPCs(true);
    }

    // Function for unfreezing all moving objects in the game
    public void UnfreezeMovingObjects()
    {
        // If we are in dialogue, don't unfreeze anything
        if (!player.GetComponent<PlayerController>().GetInDialogue())
        {
            // Let the player move and attack again
            player.GetComponent<PlayerController>().SetCanMove(true);
            player.GetComponent<PlayerController>().SetCanAttack(true);
            player.GetComponent<PlayerController>().ToggleAnimatorPause(false);

            // Unfreeze player abilities
            player.GetComponent<PlayerAbilities>().UnfreezeAbilities();

            // Unfreeze any trails, if they exist
            foreach (GameObject trail in GameObject.FindGameObjectsWithTag("Trail"))
            {
                trail.GetComponent<Trail>().ToggleAnimatorPause(false);
            }

            // Unfreeze projectiles
            UnfreezeAllArrows();

            // Unfreeze all enemies
            FreezeEnemies(false);

            // Unfreeze all NPCs
            FreezeNPCs(false);
        }
    }

    // Function for freezing all arrows (I.e. during a pause)
    public void FreezeAllArrows()
    {
        foreach (GameObject arrow in GameObject.FindGameObjectsWithTag("Arrow"))
        {
            arrow.GetComponent<Projectile>().FreezeVelocity();
        }
    }

    // Function for unfreezing all arrows (I.e. after a pause is over)
    public void UnfreezeAllArrows()
    {
        foreach (GameObject arrow in GameObject.FindGameObjectsWithTag("Arrow"))
        {
            arrow.GetComponent<Projectile>().UnfreezeVelocity();
        }
    }
    
    // Function for freezing all NPCs (by preventing their Wander script)
    public void FreezeNPCs(bool value)
    {
        foreach (GameObject npc in GetAllNPCs())
        {
            if (npc != null && npc.GetComponent<Wander>() != null)
            {
                npc.GetComponent<Wander>().ToggleNPCWandering(!value);
            }
        }
    }

    // Function to get if the player can open menus
    public bool GetCanOpenMenus()
    {
        return canOpenMenus;
    }

    // Function to set if the player can open menus
    public void SetCanOpenMenus(bool value)
    {
        canOpenMenus = value;
    }

    // Function to get if the player can open the inventory menu
    public bool GetCanOpenInventory()
    {
        return canOpenInventory;
    }

    // Function to set if the player can open the inventory menu
    public void SetCanOpenInventory(bool value)
    {
        canOpenInventory = value;
    }

    // Function to get if timers are allowed to countdown
    public bool GetCanCountdown()
    {
        return canCountdown;
    }

    // Function to set if timers are allowed to countdown
    public void SetCanCountdown(bool value)
    {
        canCountdown = value;
    }
}
