using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashFreeze : MonoBehaviour
{
    [Header("Flash Freeze Radius")]
    [SerializeField] float flashFreezeRadius = 10f;

    // Player reference
    GameObject player;

    // Private Functions

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Function that returns all enemies within the flash freeze radius
    private List<GameObject> FindEnemiesInRadius()
    {
        List<GameObject> enemiesInRange = new List<GameObject>();
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (Vector3.Distance(player.transform.position, enemy.transform.position) <= flashFreezeRadius)
            {
                enemiesInRange.Add(enemy);
            }
        }
        return enemiesInRange;
    }

    // Public Functions

    // Function that activates the Flash Freeze ability
    public void ActivateFlashFreeze(float freezeTime)
    {
        // Start the animation
        player.GetComponent<Animator>().SetTrigger("flashFreeze");

        // Freeze every enemy within range
        foreach (GameObject enemy in FindEnemiesInRadius())
        {
            if (enemy != null)
            {
                // These enemies will unfreeze themselves after this amount of time
                enemy.GetComponent<EnemyController>().SetSecondsToUnfreeze(freezeTime);

                // Freeze the enemy
                enemy.GetComponent<EnemyController>().ToggleFreeze(true);
            }
        }
    }
}
