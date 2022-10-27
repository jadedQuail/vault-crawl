using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    Rigidbody2D theRB;
    Animator projectileAnim;

    GameObject player;
    GameObject hud;

    // Only rotates on z
    [SerializeField] float upRotation;
    [SerializeField] float downRotation;
    [SerializeField] float leftRotation;
    [SerializeField] float rightRotation;

    bool seekingProjectile = false;

    float projectileSpeed;
    float projectileTimeToLive;
    Vector2 storedVelocity;

    // Bool to indicate that the projectile is disappearing (despawning)
    bool isDisappearing = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDisappearing)
        {
            // Take away health
            player.GetComponent<Health>().LoseHealth(10f);

            // Player hurt animation triggered
            player.GetComponent<Animator>().SetTrigger("playerHurt");

            // Destroy game object
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Get HUD reference
        hud = GameObject.FindWithTag("HUD");

        // Get animator reference
        projectileAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Set the projectile with the given speed (if seeking)
        if (seekingProjectile) { SetProjectile(projectileSpeed); }
            
        // Constantly store the projectile velocity - for use later during a pause (don't store 0 velocity)
        if (theRB.velocity.magnitude != 0) { storedVelocity = theRB.velocity; }

        // If there's a game pause, then the projectile needs to stop moving temporarily
        if (hud.GetComponent<HUD>().AnyMenuOpen())
        {
            ToggleAnimatorPause(true);
            theRB.velocity = new Vector2(0f, 0f);
        }
        else
        {
            ToggleAnimatorPause(false);
            theRB.velocity = storedVelocity;
        }

        // If this is a seeking projectile, have the projectile aim towards the player constantly
        if (seekingProjectile)
        {
            // Subtract time from the "time to live" value (only if no menus are open)
            if (!hud.GetComponent<HUD>().AnyMenuOpen())
            {
                projectileTimeToLive -= Time.deltaTime;
            }

            // If we're out of time, trigger this projectile's "disappear" animation
            if (projectileTimeToLive <= 0)
            {
                theRB.velocity = new Vector2(0f, 0f);
                GetComponent<Animator>().SetTrigger("disappear");
            }
        }
    }

    // Function that toggles the pause on the projectile's animator
    private void ToggleAnimatorPause(bool value)
    {
        // Pause or un-pause the animator controller
        if (value == true)
        {
            projectileAnim.speed = 0f;
        }
        else
        {
            projectileAnim.speed = 1f;
        }
    }

    // Function for getting up rotation
    public float GetUpRotation()
    {
        return upRotation;
    }

    // Function for getting down rotation
    public float GetDownRotation()
    {
        return downRotation;
    }

    // Function for getting left rotation
    public float GetLeftRotation()
    {
        return leftRotation;
    }

    // Function for getting right rotation
    public float GetRightRotation()
    {
        return rightRotation;
    }

    // Function that destroys this game object (called at the end of the "disappear" animation)
    public void DestroyThisObject()
    {
        Destroy(gameObject);
    }

    // Function for setting the projectile direction and speed - aims towards player initially, but doesn't follow
    public void SetProjectile(float speed)
    {
        // Find references for this projectile's rigidbody and the player
        theRB = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");

        // This vector always points towards the player (and it's normalized to a magnitude of 1)
        Vector2 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer = directionToPlayer.normalized;  // I don't know why I have to set it up this way, but it's the only way that works

        // Set the projectile speed
        projectileSpeed = speed;

        // Have the projectile's velocity move in the direction of the player, at the specified speed
        theRB.velocity = directionToPlayer * projectileSpeed;
    }
    // Function for setting the projectile direction and speed - manually set speed and direction
    public void SetProjectileManually(float speed, Vector3 direction)
    {
        // Find references for this projectile's rigidbody and the player
        theRB = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");

        // Set the projectile speed
        projectileSpeed = speed;

        // Have the projectile's velocity move in the direction of the player, at the specified speed
        theRB.velocity = direction * projectileSpeed;
    }

    // Function that sets whether or not this is a seeking projectile
    public void SetSeekingProjectile(bool value, float speed, float timeToLive)
    {
        seekingProjectile = value;
        projectileSpeed = speed;
        projectileTimeToLive = timeToLive;
    }

    // Function that sets whether or not the projectile is in the "disappearing" state
    public void EnableIsDisappearing()
    {
        isDisappearing = true;
    }
}
