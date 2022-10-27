using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    GameObject player;
    Rigidbody2D rb;
    float projectileDamage;
    Animator animator;

    // Only rotates on z
    [SerializeField] float upRotation;
    [SerializeField] float downRotation;
    [SerializeField] float leftRotation;
    [SerializeField] float rightRotation;

    Vector3 storedVelocity;

    // Specific whether or not this is a bomb projectile
    [SerializeField] bool isBomb;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();

        // Set direction and speed
        ActivateArrow();
    }

    // PRIVATE FUNCTIONS
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Health>().LoseHealth(projectileDamage);

            // Hit animation - but only if the enemy is currently frozen
            if (!other.GetComponent<EnemyController>().GetFrozenByAbility())
            {
                other.GetComponent<EnemyController>().GetEnemyAnimator().SetTrigger("hit");
            }

            if (isBomb)
            {
                // Blow up the projectile
                animator.SetTrigger("explode");

                // Cut the velocity
                rb.velocity = new Vector3(0f, 0f, 0f);
            }
            else
            {
                // No explosion; destroy the object right away
                Destroy(gameObject);
            }
        }

        if (other.CompareTag("Solid Object"))
        {
            if (isBomb)
            {
                // Blow up the projectile
                animator.SetTrigger("explode");

                // Cut the velocity
                rb.velocity = new Vector3(0f, 0f, 0f);
            }
            else
            {
                // No explosion; destroy the object right away
                Destroy(gameObject);
            }
        }
    }

    // Function that sets the arrow's active speed and direction
    private void ActivateArrow()
    {
        // Set direction and speed
        if (player.GetComponent<PlayerController>().GetProjectileDirection() == "up")
        {
            rb.velocity = new Vector3(0f, 1f, 0f) * speed;
        }
        else if (player.GetComponent<PlayerController>().GetProjectileDirection() == "down")
        {
            rb.velocity = new Vector3(0f, -1f, 0f) * speed;
        }
        else if (player.GetComponent<PlayerController>().GetProjectileDirection() == "right")
        {
            rb.velocity = new Vector3(1f, 0f, 0f) * speed;
        }
        else if (player.GetComponent<PlayerController>().GetProjectileDirection() == "left")
        {
            rb.velocity = new Vector3(-1f, 0f, 0f) * speed;
        }
    }

    // PUBLIC FUNCTIONS
    
    // Function for temporarily freezing the arrow's velocity, and storing its original one
    public void FreezeVelocity()
    {
        // Store the velocity
        storedVelocity = rb.velocity;

        // Now freeze the velocity
        rb.velocity = new Vector3(0f, 0f, 0f);
    }

    // Function for unfreezing the arrow's velocity, and restoring its original one
    public void UnfreezeVelocity()
    {
        // Restore the old velocity (but only if it's not zero)
        if (storedVelocity != Vector3.zero)
        {
            rb.velocity = storedVelocity;
        }
    }

    // Function for setting the arrow damage
    public void SetProjectileDamage(float damage)
    {
        projectileDamage = damage;
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

    // Function for the animation event on some projectiles
    public void EndExplosion()
    {
        // Explosion is over; destroy the game object
        Destroy(gameObject);
    }
}
