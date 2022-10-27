using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRoot : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] float rotateSpeed;
    [SerializeField] float laserDamageInterval;
    float laserCounter;

    bool playerInLasers = false;

    // GameObject refs
    GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        laserCounter = laserDamageInterval;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the root to rotate laser beams
        transform.Rotate(0, 0, Time.deltaTime * rotateSpeed, Space.Self);

        if (playerInLasers) { TakeLaserDamage(); }
        else { laserCounter = laserDamageInterval; }
    }

    // Function that sets whether or not the player is in a laser
    public void SetPlayerInLasers(bool value)
    {
        playerInLasers = value;
    }

    // Function that gives the player laser damage
    private void TakeLaserDamage()
    {
        if (laserCounter >= laserDamageInterval)
        {
            // Take damage - Lose 100 health for now
            player.GetComponent<Health>().LoseHealth(100f);
            player.GetComponent<Animator>().SetTrigger("playerHurt");
        }

        // Count down
        laserCounter -= Time.deltaTime;

        if (laserCounter <= 0f)
        {
            // Reset the timer
            laserCounter = laserDamageInterval;
        }
    }
}
