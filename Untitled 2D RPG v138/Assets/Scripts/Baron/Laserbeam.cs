using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laserbeam : MonoBehaviour
{
    // Indicate player should take damage when they enter the laserbeam
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Indicate the player is in a laserbeam, so the root can deal damage
            GetComponentInParent<LaserRoot>().SetPlayerInLasers(true);
        }
    }

    // Indicate player should stop taking damage when they exit the laserbeam
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Indicate the player is not in a laserbeam, so the root can stop dealing damage
            if (GetComponentInParent<LaserRoot>() != null)
            {
                GetComponentInParent<LaserRoot>().SetPlayerInLasers(false);
            }
        }
    }
}
