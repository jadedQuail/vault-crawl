using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trailblazer : MonoBehaviour
{
    [Header("Trailblazer Fire GameObject")]
    [SerializeField] GameObject trailFire;

    // Player's flag for whether or not Trailblazer is active
    bool isTrailblazed = false;

    // Timers and counters for Trailblazer
    float fireTimer = 0.05f;
    float fireCounter;

    // Player reference
    GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");

        // Initialize fireCounter for Trailblazer
        fireCounter = fireTimer;
    }

    private void Update()
    {
        // Check for isTrailblazed, spawn fire.
        if (isTrailblazed)
        {
            SpawnFire();
        }
    }

    // Public Functions

    // Function that activates the Trailblazer ability for the player
    public void ActivateTrailblazer()
    {
        // Set the player's trailblazer flag to true
        isTrailblazed = true;

        // ^ When this is set to be true, this script will automatically start spawning fire
    }

    // Function that deactivates the Trailblazer ability for the player
    public void DeactivateTrailblazer()
    {
        // Set the player's trailblazer flag to false
        isTrailblazed = false;

        // Reset the fire counter again
        fireCounter = fireTimer;
    }

    // Function that spawns the fire from Trailblazer, on a timer
    public void SpawnFire()
    {
        fireCounter -= Time.deltaTime;
        if (fireCounter <= 0)
        {
            Instantiate(trailFire, player.transform.position, player.transform.rotation);
            fireCounter = fireTimer;
        }
    }

    // Function that gets the player's Trailblazer flag
    public bool GetIsTrailblazed()
    {
        return isTrailblazed;
    }
}
