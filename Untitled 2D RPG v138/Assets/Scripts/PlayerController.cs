using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    GameObject hud;
    GameObject gameController;
    GameObject mainCamera;
    GameObject abilityManager;

    [SerializeField] Vector3 townSpawnPos;

    [SerializeField] float moveSpeed;
    [SerializeField] GameObject playerWeapon;

    [SerializeField] GameObject playerBow;
    [SerializeField] GameObject firePoint;
    [SerializeField] GameObject arrow;

    [SerializeField] float meleeCooldownTime = 0.35f;
    [SerializeField] float projectileCooldownTime = 0.4f;

    [SerializeField] Sprite defaultBowSprite;

    // These are the two types of weapon objects for the two types of sprite (sword)
    [SerializeField] GameObject straightSprite;
    [SerializeField] GameObject kyrisesSprite;
    
    [SerializeField] GameObject straightBow;
    [SerializeField] GameObject kyrisesBow;

    // Current weapon characteristics to store
    Sprite currentWeaponSprite;
    string currentWeaponSpriteType;
    int currentWeaponDamage;

    float meleeCooldownCounter;
    float projectileCooldownCounter;

    private Quaternion projectileRotation;

    private bool weaponEnabled = false;

    private Rigidbody2D theRB;

    private Animator myAnim;
    private float prevSpeed;  // Stores the animator's speed when pausing

    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    private GameObject lockedTarget;

    private PlayerProgression playerProgression;

    private string projectileDirection;

    // Player flags
    private bool canAttack = true;
    private bool canMove = true;
    private bool inDungeon = false;
    private bool isMoving = false;

    private bool inDialogue = false;
    private bool talkingToShopkeeper = false;

    // Integer that tracks what drop position the player is at
    int dropSpot = 0;

    // Information related to the player taking trail damage
    float trailDamageInterval = 0.25f;
    float trailDamageTimer;
    bool playerInTrail = false;


    // TESTING - shows framerate
    //void OnGUI()
    //{
    //    GUI.Label(new Rect(0, 0, 100, 100), ((int)(1.0f / Time.smoothDeltaTime)).ToString());
    //}

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
        
        // Assign declared variables
        hud = GameObject.FindWithTag("HUD");
        gameController = GameObject.FindWithTag("GameController");
        abilityManager = GameObject.FindWithTag("AbilityManager");

        // Initialize the player's trail damage timer
        trailDamageTimer = trailDamageInterval;

        // Because Unity messes this up sometimes
        canAttack = true;
        canMove = true;

        theRB = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        playerProgression = GetComponent<PlayerProgression>();

        // Set the player to have the right animations on spawn
        SetSpawnAnimations();

        meleeCooldownCounter = 0f;
        projectileCooldownCounter = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is moving, reset the drop spot
        if (theRB.velocity.x > 0f || theRB.velocity.y > 0f || theRB.velocity.x < 0f || theRB.velocity.y < 0f)
        {
            dropSpot = 0;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // If the player is in dialogue, continuously set the velocity to be 0
        // Takes care of an edge case where the player gets "launched" during dialogue
        if (inDialogue) { StopVelocity(); }

        FindCamera();

        Movement();
        Attack();
        EngageInDialogue();
        MeleeCooldown();
        ProjectileCooldown();

        // Have the player take trail damage if they're in a trail
        if (playerInTrail) { TakeTrailDamage(); }

        // Have the GameController find all enemies
        gameController.GetComponent<GameController>().FindEnemies();
    }

    // PRIVATE FUNCTIONS

    // Function that finds the camera in the new scene
    private void FindCamera()
    {
        // Find the camera
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindWithTag("MainCamera");
        }
    }

    // Function that moves the player
    private void Movement()
    {
        if (canMove)
        {
            // Diagonal movement
            if (Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") != 0)
            {
                //This accounts for extra speed from moving along the hypotenuse (it gets eliminated by this code)
                theRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * moveSpeed * (1 / new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude);
            }
            else // Non-diagonal movement
            {
                theRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * moveSpeed;
            }

            // Update the player's walking and attacking animations
            UpdatePlayerAnimation();

            // Keep the player within the map (only if we're not in a dungeon)
            if (!inDungeon)
            {
                ClampPlayer();
            }   
        }
    }

    // Function that updates the animator's values for player walking and idling
    private void UpdatePlayerAnimation()
    {
        // Favor the animations for the x axis
        if (Mathf.Abs(theRB.velocity.x) > 0)
        {
            myAnim.SetFloat("moveX", theRB.velocity.x);
            myAnim.SetFloat("moveY", 0f);
        }
        else
        {
            myAnim.SetFloat("moveX", 0f);
            myAnim.SetFloat("moveY", theRB.velocity.y);
        }
  
        // Set idling animation (last move)
        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            myAnim.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal"));
            myAnim.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical"));

            // Also set the attack direction

            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == Mathf.Abs(Input.GetAxisRaw("Vertical")))
            {
                // Favor animations for the x axis
                myAnim.SetFloat("attackX", Input.GetAxisRaw("Horizontal"));
                myAnim.SetFloat("bowAttackX", Input.GetAxisRaw("Horizontal"));
                myAnim.SetFloat("attackY", 0f);
                myAnim.SetFloat("bowAttackY", 0f);
            }
            else
            {
                myAnim.SetFloat("attackX", Input.GetAxisRaw("Horizontal"));
                myAnim.SetFloat("bowAttackX", Input.GetAxisRaw("Horizontal"));
                myAnim.SetFloat("attackY", Input.GetAxisRaw("Vertical"));
                myAnim.SetFloat("bowAttackY", Input.GetAxisRaw("Vertical"));
            }
        }
    }

    // Function that clamps the player within the bounds of the map
    private void ClampPlayer()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x),
                                         Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y),
                                         transform.position.z);
    }

    // Function for player attacking
    private void Attack()
    {
        // Main Weapon
        if (Input.GetButtonDown("Fire1"))
        {
            if (canAttack)
            {
                if (FindEnemyInRange() != null)
                {
                    // Snap to that enemy
                    SnapToEnemy(FindEnemyInRange());

                    // Set that enemy to be the locked target
                    lockedTarget = FindEnemyInRange();

                    // Start the attack animation, if possible
                    if (meleeCooldownCounter <= 0f)
                    {
                        myAnim.SetTrigger("doAttack");
                        meleeCooldownCounter = meleeCooldownTime;
                    }
                }
                else // Still let the player do an attack motion if the enemy is not in range
                {
                    // If the player is swinging and no enemy is in range, we need to reset the "locked target"
                    ResetLockedTarget();

                    // Start the attack animation, if possible
                    if (meleeCooldownCounter <= 0f)
                    {
                        myAnim.SetTrigger("doAttack");
                        meleeCooldownCounter = meleeCooldownTime;
                    }
                }
            }
        }

        // Bow
        if (canAttack)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                // Fire bow if timing permits
                if (projectileCooldownCounter <= 0f)
                {
                    myAnim.SetTrigger("doAttackBow");
                    DetermineFirePoint(arrow);
                    FireBow();
                    projectileCooldownCounter = projectileCooldownTime;
                }
            }
        }
    }

    // Function that does the cooldown for melee attacks
    private void MeleeCooldown()
    {
        // Start subtracting time from the melee counter, if that's allowed
        if (gameController.GetComponent<GameController>().GetCanCountdown())
        {
            if (meleeCooldownCounter > 0f)
            {
                meleeCooldownCounter -= Time.deltaTime;
            }
            else
            {
                meleeCooldownCounter = 0f;
            }
        }
    }

    // Function that does the cooldown for bow attacks
    private void ProjectileCooldown()
    {
        // Start subtracting time from the melee counter, if that's allowed
        if (gameController.GetComponent<GameController>().GetCanCountdown())
        {
            if (projectileCooldownCounter > 0f)
            {
                projectileCooldownCounter -= Time.deltaTime;
            }
            else
            {
                projectileCooldownCounter = 0f;
            }
        }
    }

    // Function that will find the first enemy in range and return it; or, return nothing
    // if there is no enemy in range.
    private GameObject FindEnemyInRange()
    {
        foreach (GameObject enemy in gameController.GetComponent<GameController>().GetEnemyList())
        {
            if (enemy != null)
            {
                if (enemy.GetComponent<EnemyController>().IsInAttackRange())
                {
                    return enemy;
                }
            }
        }

        // No enemy found in range
        return null;
    }

    // Function that will snap the player's direction to that of the enemy
    private void SnapToEnemy(GameObject enemy)
    {
        float differenceInX = Mathf.Abs(enemy.transform.position.x - transform.position.x);
        float differenceInY = Mathf.Abs(enemy.transform.position.y - transform.position.y);

        bool xPriority = false;
        bool yPriority = false;
        
        // Whichever coordinate distance is greater is the axis in which the player
        // should prioritize "snapping" to
        if (differenceInX >= differenceInY)
        {
            xPriority = true;
        }
        else
        {
            yPriority = true;
        }

        // Snapping in X direction
        if (xPriority)
        {
            // Player is to the left; needs to face right
            if (transform.position.x <= enemy.transform.position.x)
            {
                myAnim.SetFloat("moveX", 0);
                myAnim.SetFloat("moveY", 0);
                myAnim.SetFloat("lastMoveY", 0);
                myAnim.SetFloat("lastMoveX", 1);
                myAnim.SetFloat("attackY", 0);
                myAnim.SetFloat("attackX", 1);
                myAnim.SetFloat("bowAttackY", 0);
                myAnim.SetFloat("bowAttackX", 1);
            }
            // Player is to the right; needs to face left
            else
            {
                myAnim.SetFloat("moveX", 0);
                myAnim.SetFloat("moveY", 0);
                myAnim.SetFloat("lastMoveY", 0);
                myAnim.SetFloat("lastMoveX", -1);
                myAnim.SetFloat("attackY", 0);
                myAnim.SetFloat("attackX", -1);
                myAnim.SetFloat("bowAttackY", 0);
                myAnim.SetFloat("bowAttackX", -1);
            }
        }

        // Snapping in Y direction
        if (yPriority)
        {
            // Player is down below; needs to face up
            if (transform.position.y <= enemy.transform.position.y)
            {
                myAnim.SetFloat("moveX", 0);
                myAnim.SetFloat("moveY", 0);
                myAnim.SetFloat("lastMoveX", 0);
                myAnim.SetFloat("lastMoveY", 1);
                myAnim.SetFloat("attackX", 0);
                myAnim.SetFloat("attackY", 1);
                myAnim.SetFloat("bowAttackX", 0);
                myAnim.SetFloat("bowAttackY", 1);
            }
            // Player is up above; needs to face down
            else
            {
                myAnim.SetFloat("moveX", 0);
                myAnim.SetFloat("moveY", 0);
                myAnim.SetFloat("lastMoveX", 0);
                myAnim.SetFloat("lastMoveY", -1);
                myAnim.SetFloat("attackX", 0);
                myAnim.SetFloat("attackY", -1);
                myAnim.SetFloat("bowAttackX", 0);
                myAnim.SetFloat("bowAttackY", -1);
            }
        }
    }

    // Function for handling user input for dialogue
    private void EngageInDialogue()
    {
        if (inDialogue)
        {
            // If buttons are open, then just left clicking won't work anymore; must hit a button
            // The functionality then goes over to the buttons
            if (!hud.GetComponent<Dialogue>().GetButtonsOpen())
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (gameController.GetComponent<GameController>().GetCanOpenMenus())
                    {
                        // End of dialogue
                        if (hud.GetComponent<Dialogue>().GetDialogueStatus() == "end")
                        {
                            // Close dialogue window
                            hud.GetComponent<HUD>().EnableBotLeftCluster(true);
                            hud.GetComponent<HUD>().EnableBotRightCluster(true);
                            hud.GetComponent<HUD>().EnableDialogueCluster(false);

                            // Re-enable the abilities bar
                            hud.GetComponent<HUD>().EnableAbilitiesBar(true); 

                            // Reset the dialogue
                            hud.GetComponent<Dialogue>().ResetDialogue();

                            // Allow the player to open inventory again
                            gameController.GetComponent<GameController>().SetCanOpenInventory(true);

                            // Set dialogue status to false, unfreeze all objects
                            SetInDialogue(false);
                            gameController.GetComponent<GameController>().UnfreezeMovingObjects();
                            GetComponent<PlayerAbilities>().UnfreezeAbilities();

                            // If we're talking to a shopkeeper, open up the buy menu
                            if (talkingToShopkeeper)
                            {
                                hud.GetComponent<HUD>().OpenBuyMenu();
                            }
                        }
                        else // Start or middle of dialogue
                        {
                            hud.GetComponent<Dialogue>().AdvanceDialogueText();
                        }
                    }
                }
            }
        }
    }

    // Function that determines the positioning of the FirePoint for the projectile
    private void DetermineFirePoint(GameObject projectile)
    {
        if (myAnim.GetFloat("lastMoveX") == 1) // Facing right
        {
            firePoint.transform.localPosition = new Vector3(0.648f, -0.269f, 0f);
            projectileRotation = Quaternion.Euler(0f, 0f, projectile.GetComponent<Projectile>().GetRightRotation());
            projectileDirection = "right";
        }
        else if (myAnim.GetFloat("lastMoveX") == -1) // Facing left
        {
            firePoint.transform.localPosition = new Vector3(-0.631f, -0.252f, 0f);
            projectileRotation = Quaternion.Euler(0f, 0f, projectile.GetComponent<Projectile>().GetLeftRotation());
            projectileDirection = "left";
        }
        else if (myAnim.GetFloat("lastMoveY") == 1) // Facing up
        {
            firePoint.transform.localPosition = new Vector3(-0.202f, 0.648f, 0f);
            projectileRotation = Quaternion.Euler(0f, 0f, projectile.GetComponent<Projectile>().GetUpRotation());
            projectileDirection = "up";
        }
        else if (myAnim.GetFloat("lastMoveY") == -1) // Facing down
        {
            firePoint.transform.localPosition = new Vector3(-0.135f, -0.689f, 0f);
            projectileRotation = Quaternion.Euler(0f, 0f, projectile.GetComponent<Projectile>().GetDownRotation());
            projectileDirection = "down";
        }
    }

    // Function that fires an arrow from the bow
    private void FireBow()
    {
        // Subtract an arrow from the player's arrow count (if we're in a dungeon)
        if (inDungeon) 
        { 
            // Has an arrow to fire
            if (GetComponent<ArrowCount>().GetArrowCount() > 0) 
            { 
                GetComponent<ArrowCount>().SubtractArrow();
                // Instantiate the arrow
                Instantiate(arrow, firePoint.transform.position, projectileRotation).GetComponent<Projectile>().SetProjectileDamage(GetComponent<PlayerProgression>().GetCurrentBowDamage());
            }

            // Update HUD
            hud.GetComponent<HUD>().SetArrowCountText(GetComponent<ArrowCount>().GetArrowCount());
        }
        else
        {
            Instantiate(arrow, firePoint.transform.position, projectileRotation).GetComponent<Projectile>().SetProjectileDamage(GetComponent<PlayerProgression>().GetCurrentBowDamage());
        }
    }

    // PUBLIC FUNCTIONS

    // Function for setting the boundaries of the map (so the player doesn't walk out of it)
    public void SetBounds(Vector3 botLeft, Vector3 topRight)
    {
        bottomLeftLimit = botLeft + new Vector3(0.5f, 1f, 0f);
        topRightLimit = topRight + new Vector3(-0.5f, -1f, 0f);
    }

    // Function for the attack animation event
    public void AttackAnimation(string message)
    {
        if (message == "attack")
        {
            // Attack the selected enemy, trigger its damage animation
            if (lockedTarget != null)
            {
                lockedTarget.GetComponent<Health>().LoseHealth(playerProgression.GetPlayerDamage());

                // If the enemy is not frozen by an ability, then do the hit animation
                if (!lockedTarget.GetComponent<EnemyController>().GetFrozenByAbility())
                {
                    lockedTarget.GetComponent<EnemyController>().GetEnemyAnimator().SetTrigger("hit");
                }
            }
        }
    }

    // Function that resets the locked target to nothing
    public void ResetLockedTarget()
    {
        lockedTarget = null;
    }

    // Function for setting the player's weapon active
    public void EquipWeapon(Sprite weaponSprite, string spriteType, int damage, bool specialPass = false)
    {
        // Store Cybil's Might bool temporarily (for ease of reading)
        bool isMighted = abilityManager.GetComponent<CybilMight>().GetIsMighted();

        // Scenario 1 - No Cybil's Might, just activating a regular weapon. Store and activate.
        if (specialPass == false && isMighted == false)
        {
            StoreCurrentWeapon(weaponSprite, spriteType, damage);
            ActivateWeapon(weaponSprite, spriteType, damage);
            return;
        }

        // Scenario 2 - We are activating Cybil's Might itself. Do not store, but activate.
        if (specialPass == false && isMighted == true)
        {
            ActivateWeapon(weaponSprite, spriteType, damage);
            return;
        }

        // Scenario 3 - We are equipping a regular weapon during Cybil's Might. Store, but do not activate.
        if (specialPass == true && isMighted == true)
        {
            StoreCurrentWeapon(weaponSprite, spriteType, damage);
            return;
        }
        
    }

    // Function that activates a weapon (actually enables damage, sprite, etc.)
    private void ActivateWeapon(Sprite weaponSprite, string spriteType, int damage)
    {
        playerWeapon.SetActive(true);

        if (spriteType == "Straight")
        {
            // Activate the straight sword
            straightSprite.SetActive(true);
            straightSprite.GetComponent<SpriteRenderer>().sprite = weaponSprite;

            // Deactivate the Kyrises sword
            kyrisesSprite.SetActive(false);
        }
        else if (spriteType == "Kyrises")
        {
            // Activate the Kyrises sword
            kyrisesSprite.SetActive(true);
            kyrisesSprite.GetComponent<SpriteRenderer>().sprite = weaponSprite;

            // Deactivate the straight sword
            straightSprite.SetActive(false);
        }
        else if (spriteType == null) // If we're passed a null value, deactivate the weapon
        {
            DeactivateWeapon();
        }

        weaponEnabled = true;

        // Adjust the player's damage levels with the newly activated weapon
        GetComponent<PlayerProgression>().SetCurrentWeaponDamage(damage);

        // Set the player's overall damage
        GetComponent<PlayerProgression>().SetPlayerDamage(damage);
    }    

    // Function that stores a weapon as the player's current one, to be activated after an ability passes
    public void StoreCurrentWeapon(Sprite weaponSprite, string spriteType, int damage)
    {
        currentWeaponSprite = weaponSprite;
        currentWeaponSpriteType = spriteType;
        currentWeaponDamage = damage;
    }

    // Function for deactivating the player's weapon
    public void DeactivateWeapon()
    {
        playerWeapon.SetActive(false);
        weaponEnabled = false;

        // Get rid of sprite and damage
        straightSprite.GetComponent<SpriteRenderer>().sprite = null;
        kyrisesSprite.GetComponent<SpriteRenderer>().sprite = null;

        currentWeaponSprite = null;
        currentWeaponSpriteType = null;
        currentWeaponDamage = 0;
        GetComponent<PlayerProgression>().SetCurrentWeaponDamage(0);
    }

    // Function for changing the player's bow
    public void ChangeBowSprite(Sprite bowSprite)
    {
        playerBow.GetComponentInChildren<SpriteRenderer>().sprite = bowSprite;
    }

    // Function for getting the direction that the projectile should be facing
    public string GetProjectileDirection()
    {
        return projectileDirection;
    }

    // Function for toggling the weapon when the bow is firing
    public void ToggleWeapon(string message)
    {
        if (message == "enable")
        {
            if (weaponEnabled)
            {
                playerWeapon.SetActive(true);
            }
            else
            {
                playerWeapon.SetActive(false);
            }
        }

        if (message == "disable")
        {
            playerWeapon.SetActive(false);
        }
    }

    // Function for enabling/disabling the player's movement
    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    // Function for enabling/disabling the player from attacking
    public void SetCanAttack(bool value)
    {
        canAttack = value;
    }

    // Function for enabling/disabling the player's dialogue status
    public void SetInDialogue(bool value)
    {
        inDialogue = value;

        // Freeze every NPC from wandering, or unfreeze, if we're in/out of dialogue
        gameController.GetComponent<GameController>().FreezeNPCs(inDialogue);
    }

    // Function for killing the velocity of the player
    public void StopVelocity()
    {
        theRB.velocity = new Vector3(0f, 0f, 0f);
    }

    // Function for getting whether or not the player is in dialogue
    public bool GetInDialogue()
    {
        return inDialogue;
    }

    // Function for getting the default bow sprite
    public Sprite GetDefaultBowSprite()
    {
        return defaultBowSprite;
    }

    // Function for setting whether or not the player is talking to a shopkeeper
    public void TalkingToShopKeeper(bool value)
    {
        talkingToShopkeeper = value;
    }

    // Function for activating the straight bow
    public void ActivateStraightBow()
    {
        straightBow.SetActive(true);
        kyrisesBow.SetActive(false);
    }

    // Function for activating the Kyrises bow
    public void ActivateKyrisesBow()
    {
        kyrisesBow.SetActive(true);
        straightBow.SetActive(false);
    }

    // Function for dropping an item by the player
    public void DropByPlayer(string itemName)
    {
        float offsetX = 0f;
        float offsetY = 0f;

        if (dropSpot == 0)
        {
            offsetX = 0f;
            offsetY = -1f;
        }
        else if (dropSpot == 1)
        {
            offsetX = -1f;
            offsetY = -1f;
        }
        else if (dropSpot == 2)
        {
            offsetX = 1f;
            offsetY = -1f;
        }
        else if (dropSpot == 3)
        {
            offsetX = -1f;
            offsetY = 0f;
        }
        else if (dropSpot == 4)
        {
            offsetX = 1f;
            offsetY = 0f;
        }
        else if (dropSpot == 5)
        {
            offsetX = -1f;
            offsetY = 1f;
        }
        else if (dropSpot == 6)
        {
            offsetX = 0f;
            offsetY = 1f;
        }
        else if (dropSpot == 7)
        {
            offsetX = 1f;
            offsetY = 1f;
        }

        // Pick a position and instantiate the object
        Vector2 dropPosition = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
        Instantiate(gameController.GetComponent<DropController>().GetPickup(itemName), dropPosition, transform.rotation);

        // Increment the drop spot
        dropSpot += 1;

        // Reset the drop spot
        if (dropSpot > 7)
        {
            dropSpot = 0;
        }
    }

    // Function that toggles the player's "in dungeon" flag
    public void IsInDungeon(bool value)
    {
        inDungeon = value;
    }

    // Function that returns whether or not the player is in a dungeon
    public bool GetInDungeon()
    {
        return inDungeon;
    }

    // Function that returns the player's town spawn position
    public Vector3 GetTownSpawnPos()
    {
        return townSpawnPos;
    }

    // Function that sets the player's spawn animations
    public void SetSpawnAnimations()
    {
        // Facing downward, other animations set to downward initially
        myAnim.SetFloat("moveX", 0);
        myAnim.SetFloat("moveY", 0);
        myAnim.SetFloat("lastMoveX", 0);
        myAnim.SetFloat("lastMoveY", -1);
        myAnim.SetFloat("attackX", 0);
        myAnim.SetFloat("attackY", -1);
        myAnim.SetFloat("bowAttackX", 0);
        myAnim.SetFloat("bowAttackY", -1);
    }

    // Function that gets the player's current weapon sprite
    public Sprite GetCurrentWeaponSprite()
    {
        return currentWeaponSprite;
    }

    // Function that gets the player's current weapon sprite type
    public string GetCurrentWeaponSpriteType()
    {
        return currentWeaponSpriteType;
    }

    // Function that gets the player's current weapon damage
    public int GetCurrentWeaponDamage()
    {
        return currentWeaponDamage;
    }

    // Function that gets whether or not the player's weapon is enabled
    public bool GetWeaponEnabled()
    {
        return weaponEnabled;
    }

    // Function that gets the player's move speed
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    // Function that sets the player's move speed
    public void SetMoveSpeed(float value)
    {
        moveSpeed = value;
    }

    // Function that gets whether or not the player is moving
    public bool GetIsMoving()
    {
        return isMoving;
    }

    // Function that sets whether or not the player is in a trail
    public void SetPlayerInTrail(bool value)
    {
        playerInTrail = value;

        // If the player is out of a trail trigger, reset the damage timer
        if (!playerInTrail)
        {
            trailDamageTimer = trailDamageInterval;
        }
    }

    // Function used for giving the player trail damage
    public void TakeTrailDamage()
    {
        if (trailDamageTimer >= trailDamageInterval)
        {
            // Just 10 health lost, for now
            GetComponent<Health>().LoseHealth(10f);

            // Player hurt animation triggered
            GetComponent<Animator>().SetTrigger("playerHurt");
        }

        trailDamageTimer -= Time.deltaTime;
        if (trailDamageTimer <= 0f)
        {
            trailDamageTimer = trailDamageInterval;
        }
    }

    // Function that toggles the pause on the player's animator
    public void ToggleAnimatorPause(bool value)
    {
        // Pause or un-pause the animator controller
        if (value == true)
        {
            myAnim.speed = 0f;
        }
        else
        {
            myAnim.speed = 1f;
        }
    }
}