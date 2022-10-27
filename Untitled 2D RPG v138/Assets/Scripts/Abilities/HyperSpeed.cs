using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperSpeed : MonoBehaviour
{
    [Header("Speed Dot GameObject")]
    [SerializeField] GameObject speedDot;

    // Timers and counters for Hyper Speed
    float speedDotTimer = 0.05f;
    float speedCounter;

    // Float for storing player's normal speed
    float normalMoveSpeed;

    // Flag for storing player's hyper speed status
    bool isHyper = false;

    // Player reference
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        // Initialize speedCounter for Hyper Speed
        speedCounter = speedDotTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for isHyper, spawn speed dots.
        if (isHyper)
        {
            SpawnSpeedDot();
        }
    }

    // Public Functions

    public void ActivateHyperSpeed()
    {
        // Set the player's hyperspeed flag to true
        isHyper = true;

        // Increase the player's speed
        normalMoveSpeed = player.GetComponent<PlayerController>().GetMoveSpeed();
        player.GetComponent<PlayerController>().SetMoveSpeed(15f);

        // ^ When isHyper is set to be true, this script will start spawning speed dots
    }

    public void DeactivateHyperSpeed()
    {
        // Set the player's hyperspeed flag to false
        isHyper = false;

        // Reset the speed counter
        speedCounter = speedDotTimer;

        // Decrease the player's speed
        player.GetComponent<PlayerController>().SetMoveSpeed(normalMoveSpeed);

        // ^ When isHyper is set to be false, this script will stop spawning speeed dots
    }

    // Function that spawns a speed visual from Hyper Speed, on a timer
    public void SpawnSpeedDot()
    {
        speedCounter -= Time.deltaTime;
        if (speedCounter <= 0)
        {
            Instantiate(speedDot, player.transform.position, player.transform.rotation);
            speedCounter = speedDotTimer;
        }
    }

    // Get the player's Hyper Speed boolean
    public bool GetIsHyper()
    {
        return isHyper;
    }
}
