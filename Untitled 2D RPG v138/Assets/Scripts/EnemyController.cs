using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator enemyAnim;

    [Header("Enemy Statistics")]
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float interactRange = 5.0f;
    [SerializeField] float stopRange = 0.8f;
    [SerializeField] float enemySpeed = 7f;
    [SerializeField] float enemyAttack = 5f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] int enemyXP = 20;
    float attackCooldownCounter;

    [Header("Coin Drop")]
    [SerializeField] GameObject coin;
    [SerializeField] int coinValue;

    // Flag that indicates whether or not this enemy drops a cluster of coins when killed
    [SerializeField] bool dropsClusterCoins;

    [SerializeField] int manaReward;

    [Header("Enemy Has Drops")]
    [SerializeField] bool enemyHasDrops = true;

    [Header("Enemy Type")]
    [SerializeField] string enemyType;

    [Header("Baron")]
    [SerializeField] bool isBaron;

    GameObject player;
    GameObject gameController;
    GameObject abilityManager;

    bool inAttackRange = false;
    bool canMove = true;
    bool canAttack = true;
    bool enemyActivated = false;
    bool enemyInImmediateRange = false;  // This one is when the enemy stops moving, because they're "on top" of the player

    bool frozenByAbility = false;
    float secondsToUnfreeze;

    // Flag for the time in between when an enemy has lost its health and is truly deleted
    bool shuttingDown = false;

    Rigidbody2D enemyRB;

    Vector2 speedVector;

    DungeonDrops dungeonDrops;
    GameObject[] drops;
    int[] weights; // Must be between 0 and 1 (i.e. 0.50 is 50% of drops)
    float noDropOdds; // Probability of nothing dropping

    // For when the enemy stands in fire
    bool enemyInFlame = false;
    float damageInterval = 0.25f;
    float diCounter;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize damage interval counter for flame damage
        diCounter = damageInterval;

        // References for Rigidbody2D and Animator
        enemyRB = GetComponent<Rigidbody2D>();

        canAttack = true;

        player = GameObject.FindWithTag("Player");
        gameController = GameObject.FindWithTag("GameController");
        abilityManager = GameObject.FindWithTag("AbilityManager");

        // Set the enemy animation at the beginning so that the enemy is facing downward
        enemyAnim.SetFloat("moveY", -1f);

        attackCooldownCounter = 0f;

        // Obtain the drops, weights, and odds of a null drop from the dungeon's DungeonManager
        if (GameObject.FindWithTag("DungeonManager") != null)
        {
            (drops, weights, noDropOdds) = GameObject.FindWithTag("DungeonManager").GetComponent<DungeonDrops>().GetDrops();
        }
        // This creates a delay for health regeneration when it is first used
    }

    // Update is called once per frame
    void Update()
    {
        CheckForAttackRange();

        // Only move towards the player if he's not invisible; otherwise, sit still.
        // Bounty enemies move according to a different script
        if (!abilityManager.GetComponent<Invisibility>().GetIsInvisible() && enemyType != "bounty")
        {
            if (!isBaron)
            {
                MoveTowardsPlayer();
            }
            else
            {
                if (!GetComponentInParent<BaronAnimations>().GetBaronOverride())
                {
                    MoveTowardsPlayer();
                }
            }
        }
        else if (enemyType != "bounty")  // Bounty enemies move according to a different script)
        {
            if (!isBaron)
            {
                enemyRB.velocity = new Vector2(0f, 0f);
            }
            else
            {
                if (GetComponentInParent<BaronAnimations>() != null && !GetComponentInParent<BaronAnimations>().GetBaronOverride())
                {
                    enemyRB.velocity = new Vector2(0f, 0f);
                }
            }
        }

        if (enemyInFlame)
        {
            TakeFlameDamage(10f);
        }

        // Unfreeze enemy after some time, if necessary
        if (frozenByAbility)
        {
            secondsToUnfreeze -= Time.deltaTime;
            if (secondsToUnfreeze <= 0f)
            {
                ToggleFreeze(false);
            }
        }
    }

    // PRIVATE FUNCTIONS

    // Function that finds the distance between this enemy and another transform position
    private float GetDistance(Vector2 otherPosition)
    {
        float distance = Vector2.Distance(transform.position, otherPosition);
        return distance;
    }

    // Function that updates whether or not the player is in attack range of this particular enemy
    private void CheckForAttackRange()
    {
        // Get the distance between the player and this gameObject
        if (GetDistance(player.transform.position) <= attackRange && player != null)
        {
            inAttackRange = true;
        }
        else
        {
            inAttackRange = false;
        }
    }

    // Function for moving the enemy towards the player
    private void MoveTowardsPlayer()
    {
        float distanceBetween = Vector2.Distance(player.transform.position, transform.position);
        Vector2 directionToPlayer = player.transform.position - transform.position;

        if (distanceBetween <= stopRange)
        {
            // Indicate enemy is activated and in immediate range
            enemyActivated = true;
            enemyInImmediateRange = true;

            // Enemy is now in range of the player
            enemyRB.velocity = new Vector2(0f, 0f);

            // If the enemy can attack, then attack the player
            if (canAttack && (enemyType == "normal" || enemyType == "trailer")) { AttackPlayer(); }
        }
        else if (distanceBetween <= interactRange)
        {
            // Indicate enemy is activated, but not in immediate range
            enemyActivated = true;
            enemyInImmediateRange = false;

            // Still in range of player; still have the attack cooldown timer run
            if (gameController.GetComponent<GameController>().GetCanCountdown())
            {
                if (attackCooldownCounter > 0f)
                {
                    attackCooldownCounter -= Time.deltaTime;
                }
            }

            if (canMove)
            {
                // Reset the enemy's speed if the play moves out of range
                if (enemyRB.velocity.magnitude == 0f)
                {
                    speedVector = new Vector2(0f, 0f);
                }   
                
                // Make the enemy face the player, normalize the velocity
                speedVector = new Vector2(directionToPlayer.x, directionToPlayer.y);

                // Set the enemy's velocity
                enemyRB.velocity = Vector3.Normalize(speedVector) * enemySpeed;

                // Adjust animations
                enemyAnim.SetFloat("moveX", directionToPlayer.x);
                enemyAnim.SetFloat("moveY", directionToPlayer.y);
            }
        }
        else
        {
            // Indicate enemy is deactivated and not in immediate range
            enemyActivated = false;
            enemyInImmediateRange = false;

            // Reset attack cooldown
            attackCooldownCounter = 0f;

            enemyRB.velocity = new Vector2(0f, 0f);
        }

        // Last check; if the enemy has been set to "cannot move", then stop the enemy's motion
        if (!canMove)
        {
            // Indicate enemy is deactivated
            enemyActivated = false;

            enemyRB.velocity = new Vector2(0f, 0f);
        }
    }

    // Function for attacking the player
    private void AttackPlayer()
    {
        // Start subtracting time from the attack counter, if that's allowed
        if (gameController.GetComponent<GameController>().GetCanCountdown())
        {
            attackCooldownCounter -= Time.deltaTime;
        }

        // If our counter has reached 0, then the enemy can attack
        if (attackCooldownCounter <= 0f)
        {
            attackCooldownCounter = attackCooldown;
            // Enemy can't attack if menus are open
            if (gameController.GetComponent<GameController>().GetCanOpenMenus())
            {
                player.GetComponent<Health>().LoseHealth(enemyAttack);
                player.GetComponent<Animator>().SetTrigger("playerHurt");
            }
        }
    }

    // Function that issues damage to the enemy if it's standing in a flame
    private void TakeFlameDamage(float flameDamage)
    {
        diCounter -= Time.deltaTime;
        if (diCounter <= 0f)
        {
            // Lose health
            GetComponent<Health>().LoseHealth(10f);

            // Hit animation - but only if the enemy is not frozen
            if (!frozenByAbility)
            {
                enemyAnim.SetTrigger("hit");
            }

            // Set counter
            diCounter = damageInterval;
        }
    }

    // PUBLIC FUNCTIONS

    // Function that returns whether or not the enemy is in attack range
    public bool IsInAttackRange()
    {
        return inAttackRange;
    }

    // Function that sets whether or not the enemy can move
    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    // Function that gets whether or not the enemy can move
    public bool GetCanMove()
    {
        return canMove;
    }

    // Function that sets whether or not the enemy can attack
    public void SetCanAttack(bool value)
    {
        canAttack = value;
    }

    // Function that gets the enemy's XP value
    public int GetEnemyXP()
    {
        return enemyXP;
    }

    // Function that gets the coin GameObject off of this enemy
    public GameObject GetCoin()
    {
        return coin;
    }

    // Function that gets the coin value off of this enemy
    public int GetCoinValue()
    {
        return coinValue;
    }

    // Function that gets the mana reward value off of this enemy
    public int GetManaReward()
    {
        return manaReward;
    }

    // Function that gets whether or not the enemy is activated
    public bool GetEnemyActivated()
    {
        return enemyActivated;
    }    

    // Function that gets whether or not the enemy is in immediate range or not
    public bool GetEnemyInImmediateRange()
    {
        return enemyInImmediateRange;
    }

    // Function that drops items (if the enemy has any)
    public void DropItems()
    {
        if (enemyHasDrops)
        {
            // Spawn a random item (with a random offset so it's not on top of the coin)
            List<int> posOrNeg = new List<int>() { -1, 1 };
            float offsetX = Random.Range(0, 0.4f) * posOrNeg[Random.Range(0, posOrNeg.Count)];
            float offsetY = Random.Range(0, 0.4f) * posOrNeg[Random.Range(0, posOrNeg.Count)];
            Vector2 dropPosition = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);

            // Instantiate GameObject
            GameObject itemToDrop = gameController.GetComponent<DropController>().GeneratePickupWithWeights(drops, weights, noDropOdds);
            if (itemToDrop != null) { Instantiate(itemToDrop, dropPosition, transform.rotation); }
        }
    }

    // Sets whether or not the enemy is in a flame
    public void SetEnemyInFlame(bool value)
    {
        enemyInFlame = value;
    }

    // Gets the enemy's shutting down boolean
    public bool GetShuttingDown()
    {
        return shuttingDown;
    }

    // Sets the enemy's shutting down boolean
    public void SetShuttingDown(bool value)
    {
        shuttingDown = value;
    }

    // Function that freezes and unfreezes only this specific enemy (for Flash Freeze)
    public void ToggleFreeze(bool value)
    {
        // Turn the frozen animation on or off
        enemyAnim.SetBool("isFrozen", value);

        // Freeze the enemy's ability to move and attack
        SetCanMove(!value);
        SetCanAttack(!value);

        // Pause or un-pause the animator controller
        ToggleAnimatorPause(value);

        // Set the enemy's "Frozen by Ability" flag to true
        frozenByAbility = value;
    }

    // Function that gets the enemy's "frozen by ability" status
    public bool GetFrozenByAbility()
    {
        return frozenByAbility;
    }

    // Function that sets seconds to unfreeze
    public void SetSecondsToUnfreeze(float seconds)
    {
        // Just passes time to this script to rundown in "Update"
        secondsToUnfreeze = seconds;
    }

    // Function that toggles the pause on the player's animator
    public void ToggleAnimatorPause(bool value)
    {
        // Pause or un-pause the animator controller
        if (value == true)
        {
            enemyAnim.speed = 0f;
        }
        else
        {
            enemyAnim.speed = 1f;
        }
    }

    // Function that gets this enemy's type
    public string GetEnemyType()
    {
        return enemyType;
    }

    // Function that gets whether or not the player is in interact range with this enemy
    public bool GetInInteractRange()
    {
        float distanceBetween = Vector2.Distance(player.transform.position, transform.position);
        return distanceBetween <= interactRange;
    }

    // Function that gets whether or not this enemy drops cluster coins
    public bool GetDropsClusterCoins()
    {
        return dropsClusterCoins;
    }

    // Function that gets whether or not this enemy is a baron
    public bool GetIsBaron()
    {
        return isBaron;
    }

    // Function for getting this enemy's animator
    public Animator GetEnemyAnimator()
    {
        return enemyAnim;
    }

    // Collision detection for Baron charges
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && isBaron && GetComponentInParent<BaronAnimations>().GetInChargeState())
        {
            // Play an impact explosion animation
            GameObject.FindWithTag("ChargeExplosion").GetComponent<Animator>().SetTrigger("explode");

            // Have the player lose health
            other.gameObject.GetComponent<Health>().LoseHealth(GetComponentInParent<BaronAnimations>().GetChargeDamange());

            // Remove the baron from the "charge state" to ensure the player only takes damage once
            GetComponentInParent<BaronAnimations>().SetInChargeState(false);
        }
    }
}
