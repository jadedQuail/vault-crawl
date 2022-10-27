using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronAnimations : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator enemyAnim;

    [Header("Lasers")]
    [SerializeField] GameObject laserRoot;
    [SerializeField] GameObject laserCharging;

    [Header("")]
    [SerializeField] Rigidbody2D baronRB;
    [SerializeField] float chargeSpeed;
    [SerializeField] float chargeDamage;
    [SerializeField] float timeBetweenStates;
    float tbsCounter;

    // State number determines which attack state the baron will enter every few seconds when it's checked
    // Total range: 1-10
    // 1-3 (30%): Charge
    int stateNumber;

    bool inChargeState = false;

    // GameObject references
    GameObject player;
    GameObject abilityManager;
    GameObject hud;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        abilityManager = GameObject.FindWithTag("AbilityManager");
        hud = GameObject.FindWithTag("HUD");
        tbsCounter = timeBetweenStates;
    }

    private void Update()
    {
        // Start cycling through phases if
        // 1.) The enemy is activated (i.e. in range of the player)
        // 2.) The player is not invisible
        // 3.) The enemy is not frozen by ability
        // 4.) No menus are open
        if (GetComponentInChildren<EnemyController>().GetEnemyActivated() && !abilityManager.GetComponent<Invisibility>().GetIsInvisible()
            && !GetComponentInChildren<EnemyController>().GetFrozenByAbility() && !hud.GetComponent<HUD>().AnyMenuOpen())
        {
            // Call a new stateNumber every few seconds
            DecideState();

            // Charge
            if (stateNumber >= 1 && stateNumber <= 3)
            {
                // Nullify the stateNumber by setting it to zero
                stateNumber = 0;

                // Charge attack
                enemyAnim.SetTrigger("charge");
                GetComponentInChildren<EnemyFire>().SetCanFire(false);
            }

            if (stateNumber >= 4 && stateNumber <= 6)
            {
                // Nullify the stateNumber by setting it to zero
                stateNumber = 0;

                // Laser attack
                enemyAnim.SetTrigger("laser");
                GetComponentInChildren<EnemyFire>().SetCanFire(false);
            }
        }
    }

    // Function that generates a random number every few seconds that decides what attack the baron does
    private void DecideState()
    { 
        tbsCounter -= Time.deltaTime;
        if (tbsCounter <= 0)
        {
            // Generate a state, 1 through 10 inclusive
            stateNumber = Random.Range(1, 11);
            tbsCounter = timeBetweenStates;
        }
    }

    // This flag gives control of the Rigidbody to this script
    bool baronOverride = false;

    // Function that sets the stopVelocity flag to true
    public void EnableBaronOverride()
    {
        // Indicate that the baron override is in place
        baronOverride = true;

        // Set the baron's velocity to 0 at first
        baronRB.velocity = new Vector2(0f, 0f);
    }

    // Function that sets the stopVelocity flag to false
    public void DisableBaronOverride()
    {
        // Indicate that the baron override is finished
        baronOverride = false;
    }

    // Function that gets the status of the baron override
    public bool GetBaronOverride()
    {
        return baronOverride;
    }

    // Function that starts the charge animation
    public void StartCharge()
    {
        inChargeState = true;

        // Find the direction to the player
        Vector2 directionToPlayer = (player.transform.position - transform.GetChild(0).transform.position).normalized * chargeSpeed;

        // Have the baron charge in that direction
        baronRB.velocity = new Vector2(directionToPlayer.x, directionToPlayer.y);
    }

    // Function that ends the charge animation
    public void EndCharge()
    {
        inChargeState = false;

        // Re-enable firing
        GetComponentInChildren<EnemyFire>().SetCanFire(true);

        // Disable the baron override
        DisableBaronOverride();
    }

    // Function that begins the laser charge-up phase
    public void ActivateLaserChargeUp()
    {
        // Baron should stop moving during this phase
        EnableBaronOverride();

        // Laser charging GameObject needs to be active
        laserCharging.SetActive(true);
    }

    // Function that begins the laser firing
    public void ActivateLasers()
    {
        // Laser root needs to be activated; laser charging needs to be deactivated
        laserCharging.SetActive(false);
        laserRoot.SetActive(true);

        // When the laserRoot first starts up, make sure we start at "false" for the player being in the lasers
        laserRoot.GetComponent<LaserRoot>().SetPlayerInLasers(false);

        // Baron can start moving according to normal enemy movement again
        DisableBaronOverride();
    }

    // Function that ends the laser firing
    public void DeactivateLasers()
    {
        // Disable the laser root
        if (laserRoot != null) { laserRoot.SetActive(false); }
    }

    // Function that indicates whether or not this baron is in charge state
    public bool GetInChargeState()
    {
        return inChargeState;
    }

    // Function that sets whether or not the baron is in the charge state
    public void SetInChargeState(bool value)
    {
        inChargeState = value;
    }

    // Function that gets the charge damage
    public float GetChargeDamange()
    {
        return chargeDamage;
    }

    // Function that toggles the pause on the baron's animator
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
}
