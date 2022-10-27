using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator enemyAnim;

    
    private EnemyController enemyController;

    [Header("")]
    public float timeBetweenProjectiles;
    private float timer;

    private Quaternion projectileRotation;

    public GameObject projectile;

    [Header("Projectile Speed")]
    [SerializeField] float speed;

    [Header("Cluster Shot")]
    [SerializeField] bool isCluster;

    [Header("Projectile Time to Live")]
    [SerializeField] float timeToLive;

    // Other object references
    GameObject player;
    GameObject abilityManager;

    // Boolean for allowing firing
    bool canFire = false;

    void Start()
    {
        canFire = true;

        enemyController = GetComponent<EnemyController>();

        timer = timeBetweenProjectiles;

        // Initialize object references
        player = GameObject.FindWithTag("Player");
        abilityManager = GameObject.FindWithTag("AbilityManager");
    }

    // Update is called once per frame
    void Update()
    {
        // Enemy can fire when activated, but not when in the immediate range of the player or if the player is invisible
        // Enemy cannot fire when:
        //    1. Inactivated
        //    2. Is too close to the player
        //    3. The player is invisible
        //    4. This is a Baron and they are charging
        if (enemyController.GetEnemyActivated() && !enemyController.GetEnemyInImmediateRange() 
            && !abilityManager.GetComponent<Invisibility>().GetIsInvisible())
        {
            // Timer pauses when canFire is false
            if (canFire) { timer -= Time.deltaTime; }
            if (timer <= 0f)
            {
                // Fire projectiles
                FireProjectile();

                // Reset the timer
                timer = timeBetweenProjectiles;
            }
        }
    }

    // PRIVATE FUNCTIONS //

    // Function that fires a projectile
    private void FireProjectile()
    {
        DetermineProjectileRotation(projectile);

        if (GetComponent<EnemyController>().GetEnemyType() == "sentry") // Sentries fire heat-seeking projectiles
        {
            Instantiate(projectile, transform.position, projectileRotation).GetComponent<EnemyProjectile>().SetSeekingProjectile(true, speed, timeToLive);
        }
        else // Everyone else does not
        {
            if (isCluster)
            {
                // Find the direction towards the player
                Vector3 directionToPlayer = player.transform.position - transform.position;
                directionToPlayer = directionToPlayer.normalized;

                // Create two more vectors that are offset by 15 degrees
                Vector3 clusterShot1 = Quaternion.AngleAxis(5, Vector3.forward) * directionToPlayer;
                Vector3 clusterShot2 = Quaternion.AngleAxis(-5, Vector3.forward) * directionToPlayer;

                // Instatiate all these shots, manually
                Instantiate(projectile, transform.position, projectileRotation).GetComponent<EnemyProjectile>().SetProjectileManually(speed, directionToPlayer);
                Instantiate(projectile, transform.position, projectileRotation).GetComponent<EnemyProjectile>().SetProjectileManually(speed, clusterShot1);
                Instantiate(projectile, transform.position, projectileRotation).GetComponent<EnemyProjectile>().SetProjectileManually(speed, clusterShot2);
            }
            else
            {
                Instantiate(projectile, transform.position, projectileRotation).GetComponent<EnemyProjectile>().SetProjectile(speed);
            }
        }
    }

    // Function that determines the projectile's rotation
    private void DetermineProjectileRotation(GameObject projectile)
    {
        // Favor X
        if (Mathf.Abs(enemyAnim.GetFloat("moveX")) >= Mathf.Abs(enemyAnim.GetFloat("moveY")))
        {
            if (enemyAnim.GetFloat("moveX") > 0f) // Facing right
            {
                projectileRotation = Quaternion.Euler(0f, 0f, projectile.GetComponent<EnemyProjectile>().GetRightRotation());
            }
            else if (enemyAnim.GetFloat("moveX") <= 0f) // Facing left
            {
                projectileRotation = Quaternion.Euler(0f, 0f, projectile.GetComponent<EnemyProjectile>().GetLeftRotation());
            }
        }
        // Favor Y
        else
        {
            if (enemyAnim.GetFloat("moveY") > 0f) // Facing up
            {
                projectileRotation = Quaternion.Euler(0f, 0f, projectile.GetComponent<EnemyProjectile>().GetUpRotation());
            }
            else if (enemyAnim.GetFloat("moveY") <= 0f) // Facing down
            {
                projectileRotation = Quaternion.Euler(0f, 0f, projectile.GetComponent<EnemyProjectile>().GetDownRotation());
            }
        }
    }

    // PRIVATE FUNCTIONS //

    // Function for getting whether or not the enemy can fire
    public bool GetCanFire()
    {
        return canFire;
    }

    // Function for setting whether or not the enemy can fire
    public void SetCanFire(bool value)
    {
        canFire = value;
    }
}
