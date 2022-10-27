using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [Header("Active Time")]
    [SerializeField] float activeTime;
    [SerializeField] string targetTag;
    [SerializeField] bool enemyTrail;

    [Header("Animator")]
    [SerializeField] Animator anim;

    // Indicates whether or not this trail is starting to disappear
    bool isDisappearing = false;

    // HUD reference
    GameObject hud;

    // Reference to the trailer enemy that spawned this trail
    GameObject enemySpawnedFrom;

    private void Start()
    {
        hud = GameObject.FindWithTag("HUD");
    }

    private void Update()
    {
        // Only countdown to despawn if there are no menus open
        if (!hud.GetComponent<HUD>().AnyMenuOpen_NotIncludingVictory())
        {
            activeTime -= Time.deltaTime;
        }

        // Enemy trails despawn when time is up or the enemy has been killed
        if (enemyTrail && (activeTime <= 0f || enemySpawnedFrom == null))
        {
            // Trigger the disappearing animation (which ultimately leads to destruction)
            GetComponent<Animator>().SetTrigger("disappear");
        }

        // Player trails despawn when time is up
        if (!enemyTrail && activeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    // If the desired target interacts with the trail, do damage
    private void OnTriggerStay2D(Collider2D other)
    {
        // Enemy - standing in flames
        if (other.CompareTag("Enemy") && targetTag == "Enemy")
        {
            GameObject enemy = other.gameObject;
            enemy.GetComponent<EnemyController>().SetEnemyInFlame(true);
        }

        // Player - standing in trail (if this trail is disappearing, however, it cannot hurt the player)
        if (other.CompareTag("Player") && targetTag == "Player" && !isDisappearing)
        {
            // Player reference
            GameObject player = other.gameObject;

            // Set the player "in the trail"
            player.GetComponent<PlayerController>().SetPlayerInTrail(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Enemy out of fire - stop taking damage
        if (other.CompareTag("Enemy") && targetTag == "Enemy")
        {
            GameObject enemy = other.gameObject;
            enemy.GetComponent<EnemyController>().SetEnemyInFlame(false);
        }

        // Player out of trail - stop taking damage
        if (other.CompareTag("Player") && targetTag == "Player")
        {
            GameObject player = other.gameObject;
            player.GetComponent<PlayerController>().SetPlayerInTrail(false);
        }
    }

    // Function that sets the enemy that this trail spawned from
    public void SetEnemySpawnedFrom(GameObject enemy)
    {
        enemySpawnedFrom = enemy;
    }

    // Function that indicates that this trail is in the process of disappearing
    public void EnableIsDisappearing()
    {
        isDisappearing = true;
    }

    // Function that destroys this object
    public void DestroyThisObject()
    {
        Destroy(gameObject);
    }

    // Function that toggles whether or not the trail animation pauses
    public void ToggleAnimatorPause(bool value)
    {
        // Pause or un-pause the animator controller
        if (value == true)
        {
            anim.speed = 0f;
        }
        else
        {
            anim.speed = 1f;
        }
    }
}