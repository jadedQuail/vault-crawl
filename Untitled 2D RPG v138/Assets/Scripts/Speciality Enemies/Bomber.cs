using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator enemyAnim;

    [Header("Bomber Parameters")]
    [SerializeField] float triggerRange;
    [SerializeField] float explosionRange;
    [SerializeField] GameObject[] smokes;
    [SerializeField] float bomberDamage;

    // Other GameObject references
    GameObject player;
    GameObject gameController;
    GameObject abilityManager;

    // Boolean to indicate that this bomber enemy is shutting down
    bool bomberShuttingDown;

    private void Start()
    {
        // Initialize other GameObject references
        player = GameObject.FindWithTag("Player");
        gameController = GameObject.FindWithTag("GameController");
        abilityManager = GameObject.FindWithTag("AbilityManager");
    }

    private void Update()
    {
        // Constantly check to see if we're in range for explosion
        CheckForExplosion();

        // If the bomber is shutting down, nullify it every frame
        if (bomberShuttingDown)
        {
            // Remove from player's list of enemies
            GameObject[] enemyArray = gameController.GetComponent<GameController>().GetEnemyList();
            int indexValue = System.Array.IndexOf(enemyArray, gameObject);
            gameController.GetComponent<GameController>().NullifyEnemyAtIndex(indexValue);
        }
    }

    // Function that triggers an explosion when the player gets too close
    private void CheckForExplosion()
    {
        // Find the distance between the player and this bomber
        float distanceBetween = Vector2.Distance(player.transform.position, transform.position);
        
        // Close enough for explosion, which should be triggered
        if (distanceBetween < triggerRange && !abilityManager.GetComponent<Invisibility>().GetIsInvisible())
        {
            Explode();
        }
    }

    // Function that sets off the actual explosion
    private void Explode()
    {
        // Stop this enemy's movement
        GetComponent<EnemyController>().SetCanMove(false);

        // Start explosion animation
        enemyAnim.SetTrigger("explode");
    }

    // Function that gets called at the end of the bomber's explosion, to destroy the object
    public void DestroyThisObject()
    {
        Destroy(gameObject);
    }

    // Function that gets called at the end of the bomber's expansion, which starts the full shutdown of the enemy
    public void BeginShutdown()
    {
        // The explosion has begun - hurt the player, if applicable
        BombPlayer();

        foreach(GameObject smoke in smokes)
        {
            Destroy(smoke);
        }

        // Indicate that the bomber needs to shut down
        bomberShuttingDown = true;
    }

    // Function that damages the player if they are in range of the explosion
    public void BombPlayer()
    {
        // Find the distance between the player and this bomber
        float distanceBetween = Vector2.Distance(player.transform.position, transform.position);

        // See if the player is in explosion range
        if (distanceBetween < explosionRange)
        {
            // Damage the player
            player.GetComponent<Health>().LoseHealth(bomberDamage);

            // Play the player's hurt animation
            player.GetComponent<Animator>().SetTrigger("playerHurt");
        }
    }
}
